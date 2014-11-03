using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using GitCode.Models;
using ICSharpCode.SharpZipLib.Zip;
using GitCode.Base;
using GitCode.Data;

namespace GitCode.Git
{
    public class GitService : IDisposable
    {
        private readonly LibGit2Sharp.Repository _repository;
        private readonly string _repositoryPath;
        private readonly Lazy<Encoding> _i18n;
        private bool _disposed;
        private List<CommitModel> commitListRecursif = new List<CommitModel>();
        private static Dictionary<string, string> fileFound = new Dictionary<string, string>();
        private static Dictionary<string, IEnumerable<CommitChangeModel>> commitChangeHistory = new Dictionary<string, IEnumerable<CommitChangeModel>>();
        private static LibGit2Sharp.Repository repo;
        private TransferContent transferContent = new TransferContent();
        public Encoding I18n { get { return _i18n.Value; } }
        public string Name { get; private set; }
        public GitService()
        {
        }
        public GitService(string path)
        {
            if (!LibGit2Sharp.Repository.IsValid(path))
                throw new RepositoryNotFoundException(String.Format(CultureInfo.InvariantCulture, "Path '{0}' doesn't point at a valid Git repository or workdir.", path));

            _repositoryPath = path;
            _repository = new LibGit2Sharp.Repository(path);
            _i18n = new Lazy<Encoding>(() =>
            {
                var entry = _repository.Config.Get<string>("i18n.commitEncoding");
                return entry == null
                    ? null
                    : CpToEncoding(entry.Value);
            });
            Name = new DirectoryInfo(path).Name;
        }

        private Encoding CpToEncoding(string encoding)
        {
            try
            {
                if (encoding.StartsWith("cp", StringComparison.OrdinalIgnoreCase))
                    return Encoding.GetEncoding(int.Parse(encoding.Substring(2)));

                return Encoding.GetEncoding(encoding);
            }
            catch
            {
                return null;
            }
        }

        //CREATE INIT REPO
        public bool CreateRepository(string path)
        {
            try
            {
                using (var repo = new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Init(path, true)))
                {
                    repo.Config.Set("core.logallrefupdates", true);
                }
                return true;
            }
            catch
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch { }
                return false;
            }
        }

        public void InfoRefs(string service, string username, Stream inStream, Stream outStream)
        {
            Contract.Requires(service == "receive-pack" || service == "upload-pack");
            RunGitCmd(service, username, true, inStream, outStream);
        }

        public void ExecutePack(string service, string username, Stream inStream, Stream outStream)
        {
            Contract.Requires(service == "receive-pack" || service == "upload-pack");
            RunGitCmd(service, username, false, inStream, outStream);
        }

        private void RunGitCmd(string serviceName, string username, bool advertiseRefs, Stream inStream, Stream outStream)
        {
            var args = serviceName + " --stateless-rpc";
            if (advertiseRefs)
                args += " --advertise-refs";
            args += " \"" + _repositoryPath + "\"";

            var info = new System.Diagnostics.ProcessStartInfo(@transferContent.getGitDirectory(), args)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(@transferContent.getRepoDirectory()+@"\"+username),
            };

            using (var process = System.Diagnostics.Process.Start(info))
            {
                inStream.CopyTo(process.StandardInput.BaseStream);
                process.StandardInput.Close();
                process.StandardOutput.BaseStream.CopyTo(outStream);

                process.WaitForExit();
            }
        }

        public static DirectoryInfo GetDirectoryInfo(string project, string username, string repoLocation)
        {
            return new DirectoryInfo(Path.Combine(@repoLocation+@"\" + username, project)); //ubah
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (repo != null)
                    {
                        repo.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~GitService()
        {
            Dispose(false);
        }

        #region GitRepositoryDetail

        public IEnumerable<CommitModel> getCommitsInfo(string path, string branch, string content = "")
        {
            repo = new LibGit2Sharp.Repository(path);

            commitChangeHistory = new Dictionary<string, IEnumerable<CommitChangeModel>>();
            IEnumerable<CommitModel> allCommits;
            try
            {
                allCommits = repo.Branches[branchFinder(path, branch)].Commits.Select(s => new CommitModel
                {
                    CommitCode = s.Sha.Substring(0,6),
                    Author = s.Author,
                    Commiter = s.Committer,
                    CommitTime = s.Committer.When.ToString(),
                    Message = s.Message != null ? s.Message : "",
                    ShortMessage = s.MessageShort != null ? s.MessageShort : "",
                    TreeEntry = getCommitDetail(s, path, branch, content),
                    Parent = s.Parents.Select(p => p.Sha).ToArray(),
                    CommitChange = commitChangeHistory.ContainsKey(s.Sha) ? commitChangeHistory[s.Sha] : getCommitChange(s,path),
                    GitPath = userGitPath(path),
                    SHA = s.Sha,
                    Feature = getFeature(s.Message)
                }).ToList();
            }
            catch(Exception e)
            {
                allCommits = null;
            }
            return allCommits;
        }

        public string branchFinder(string path, string branchIn)
        {
            repo = new LibGit2Sharp.Repository(path);
            var p = branchIn + "/";
            Commit commitFind = null;
            Branch branchValue = repo.Branches.FirstOrDefault(bv => p.StartsWith(bv.Name + "/"));
            Tag tagvalue = repo.Tags.FirstOrDefault(bv => p.StartsWith(bv.Name + "/"));
            List<Branch> allBranch = repo.Branches.ToList();

            if (branchValue != null)
                return branchIn;
            else
            foreach (Branch branch in allBranch)
            {
                commitFind = branch.Commits.Where(c => c.Sha == tagvalue.Target.Sha).FirstOrDefault();
                branchIn = branch.CanonicalName.Split('/')[2];
                if (commitFind != null)
                    break;
            }
            return branchIn;
        }

        public Commit getFileLastCommit(Commit commit, string path, string fileName, out Commit comval, string branchName, string content ="")
        {
            if(repo == null)
                repo = new LibGit2Sharp.Repository(path);
            Commit commitValue = null;
            IEnumerable<CommitChangeModel> commitChange;

            if (commitChangeHistory.ContainsKey(commit.Sha))
            {
                commitChange = commitChangeHistory[commit.Sha];
            }
            else
            {
                commitChange = getCommitChange(commit, path, content);
                commitChangeHistory.Add(commit.Sha, commitChange);
            }

            if (commitChange == null )
            {
                comval = null;
                return commitValue;
            }
            if (commitChange.Where(cc => cc.Path == fileName).Any())
            {
                commitValue = commit;
                comval = commitValue;
                return commitValue;
            }
            else
            {
                List<string> commitParent = commit.Parents.OrderBy(cp => cp.Author.When).Select(cp => cp.Sha).ToList();
                Commit useless = null;
                Commit commitSelected =null;

                List<Branch> commitbranch = repo.Branches.ToList();

                foreach (string parent in commitParent)
                {
                    commitSelected = repo.Branches[branchFinder(path,branchName)].Commits.Where(c => c.Sha == parent).FirstOrDefault();
                    getFileLastCommit(commitSelected, path, fileName, out useless, branchName);
                    commitValue = null;
                    if (useless != null)
                    {
                        break;
                    }
                }
                comval = useless;
                return commitValue;
            }
        }

        public IEnumerable<TreeEntryModel> getCommitDetail(Commit commit, string path, string branch, string content ="")
        {
            Commit commitValue = null;
            IEnumerable<TreeEntryModel> allTree = commit.Tree.Select(s => new TreeEntryModel
                {
                    Mode = s.Mode,
                    Name = s.Name,
                    Path = s.Path,
                    Target = commit.Tree[s.Path].Target,
                    TargetType = commit.Tree[s.Path].TargetType,
                    Commit = getFileCommit(commit,path,s.Name,branch,content)
                }).ToList();

            return allTree;
        }

        private Commit getFileCommit(Commit commit, string path, string filename, string branch,string content = "")
        {
            Commit commitValue = null;
            Commit commitVal = getFileLastCommit(commit, path, filename, out commitValue, branch, content);

            if(commitVal == null)
                commitVal = commitValue;
            return commitVal;
        }

        public IEnumerable<CommitChangeModel> getCommitChange(Commit commit, string path, string content = "")
        {
            IEnumerable<CommitChangeModel> commitChangeValue = null;
                
            if (commit != null)
            {
                Patch treePatchInfo;
                var repo = new LibGit2Sharp.Repository(path);
                var compareOptions = new LibGit2Sharp.CompareOptions
                    {
                        Similarity = SimilarityOptions.Renames,
                    };
                Tree firstTree = commit.Parents.Any()
                            ? commit.Parents.FirstOrDefault().Tree
                            : null;
                Tree compareTree = commit.Tree;
                TreeChanges treeChangeInfo = repo.Diff.Compare<TreeChanges>(firstTree, compareTree, null, compareOptions: compareOptions);
                if (content != "")
                {
                    try
                    {
                        var stat = repo.Diff.Compare<Patch>(firstTree, compareTree, null, compareOptions: compareOptions);
                        commitChangeValue = from s in treeChangeInfo
                                            let patch = stat[s.Path]
                                            select new CommitChangeModel
                                            {
                                                ChangeKind = s.Status,
                                                LinesAdded = patch.LinesAdded,
                                                LinesDeleted = patch.LinesDeleted,
                                                OldPath = s.Path,
                                                Patch = "",
                                                Path = s.Path
                                            };
                    }
                    catch(Exception e)
                    {
                        commitChangeValue = from s in treeChangeInfo
                                            //let patch = treePatchInfo[s.Path]
                                            select new CommitChangeModel
                                            {
                                                ChangeKind = s.Status,
                                                LinesAdded = 0,
                                                LinesDeleted = 0,
                                                OldPath = s.Path,
                                                Patch = "",
                                                Path = s.Path
                                            };
                    }
                }
                else
                {
                    commitChangeValue = from s in treeChangeInfo
                                        //let patch = treePatchInfo[s.Path]
                                        select new CommitChangeModel
                                        {
                                            ChangeKind = s.Status,
                                            LinesAdded = 0,
                                            LinesDeleted = 0,
                                            OldPath = s.Path,
                                            Patch = "",
                                            Path = s.Path
                                        };
                }
            }
            return commitChangeValue;
        }

        public List<BranchModel> getBranch(string path)
        {
            repo = new LibGit2Sharp.Repository(path);
            List<BranchModel> allBranch = new List<BranchModel>();
            foreach (Branch branch in repo.Branches)
            {
                BranchModel branchValue = new BranchModel
                {
                    BranchName = branch.Name,
                    CommitInBranch = branch.Commits.FirstOrDefault()
                };
                allBranch.Add(branchValue);
            }

            return allBranch;
        }

        public  CommitModel getCommitBy(string branchName, string gitPath)
        {
            repo = new LibGit2Sharp.Repository(gitPath);
            var p = branchName + "/";
            //var p = "v1.4/";
            CommitModel commitValue = null;
            Commit commitFind;
            Branch branchValue = repo.Branches.FirstOrDefault(bv=> p.StartsWith(bv.Name + "/"));
            Tag tagvalue = repo.Tags.FirstOrDefault(bv => p.StartsWith(bv.Name + "/"));
            if (branchValue != null)
                commitValue = makeCommitFiller(branchValue.Tip, gitPath, branchName);
            else if (tagvalue != null)
            {
                List<Branch> allBranch = repo.Branches.ToList();
                foreach(Branch branch in allBranch)
                {
                    commitFind = branch.Commits.Where(c => c.Sha == tagvalue.Target.Sha).FirstOrDefault();
                    branchName = branch.CanonicalName.Split('/')[2];
                    if (commitFind != null)
                        break;
                }
                commitValue = makeCommitFiller((Commit)tagvalue.Target, gitPath, branchName);
            }
            return commitValue;
        }

        private List<Commit> allCommit = new List<Commit>();

        public string getCommitFile(string branchName, string gitPath, string sha = "")
        {
            string filename;
            Commit commitChoosen;
            getAllCommitBy(branchName, gitPath, sha);
            string[] pathSplitter = gitPath.Split('\\');

            if (sha != "")
            {
                commitChoosen = allCommit.Where(c => c.Sha == sha).FirstOrDefault();
                filename = pathSplitter[pathSplitter.Count()-1]+"-"+ branchName + "-" + sha.Substring(0, 6);
            }
            else
            {
                commitChoosen = allCommit.FirstOrDefault();
                filename = pathSplitter[pathSplitter.Count() - 1] + "-" + branchName;
            }
            
            using (var zipOutputStream = new ZipOutputStream(new FileStream(@transferContent.getCacheDirectory()+@"\"+filename, FileMode.Create)))
            {
                var stack = new Stack<Tree>();
                string newline = "\n";
                stack.Push(commitChoosen.Tree);
                while (stack.Count != 0)
                {
                    var tree = stack.Pop();
                    foreach (var entry in tree)
                    {
                        byte[] bytes;
                        switch (entry.TargetType)
                        {
                            case TreeEntryTargetType.Blob:
                                zipOutputStream.PutNextEntry(new ZipEntry(entry.Path));
                                var blob = (Blob)entry.Target;
                                using (var streamReader = new MemoryStream())
                                {
                                    blob.GetContentStream().CopyTo(streamReader);
                                    bytes = streamReader.ToArray();
                                }

                                if (newline == null)
                                    zipOutputStream.Write(bytes, 0, bytes.Length);
                                else
                                {
                                    var encoding = FileHelper.DetectEncoding(bytes, CpToEncoding(commitChoosen.Encoding), _i18n.Value);
                                    if (encoding == null)
                                        zipOutputStream.Write(bytes, 0, bytes.Length);
                                    else
                                    {
                                        bytes = FileHelper.ReplaceNewline(bytes, encoding, newline);
                                        zipOutputStream.Write(bytes, 0, bytes.Length);
                                    }
                                }
                                break;
                            case TreeEntryTargetType.Tree:
                                stack.Push((Tree)entry.Target);
                                break;
                            case TreeEntryTargetType.GitLink:
                                zipOutputStream.PutNextEntry(new ZipEntry(entry.Path + "/.gitsubmodule"));
                                bytes = Encoding.ASCII.GetBytes(entry.Target.Sha);
                                zipOutputStream.Write(bytes, 0, bytes.Length);
                                break;
                        }
                    }
                }
                zipOutputStream.SetComment(commitChoosen.Sha);
            }

            return filename;
        }

        private void getAllCommitBy(string path, string gitPath, string shaChoosen = "")
        {
            repo = new LibGit2Sharp.Repository(gitPath);
            //List<CommitModel> commitListByValue = new List<CommitModel>();
            var p = path + "/";
            Commit commitChoosen = null;
            Branch branchChecker = repo.Branches.Where(bv => p.StartsWith(bv.Name + "/")).FirstOrDefault();
            Commit branchValue = branchChecker != null ? branchChecker.Tip : null;
            IEnumerable<Tag> tagValue = repo.Tags.Where(bv => p.StartsWith(bv.Name + "/"));


            if (branchValue != null)
            {
                if (shaChoosen == "")
                    shaChoosen = branchChecker.Tip.Sha;
                commitChoosen = repo.Branches[path].Commits.Where(c => c.Sha == shaChoosen).FirstOrDefault();
            }
            else
            {
                List<Branch> allBranch = repo.Branches.ToList();
                foreach (Branch branch in allBranch)
                {
                    if (shaChoosen != "")
                        commitChoosen = branch.Commits.Where(c => c.Sha == tagValue.FirstOrDefault().Target.Sha).FirstOrDefault();
                    else
                    {
                        commitChoosen = branch.Commits.FirstOrDefault();
                        shaChoosen = commitChoosen.Sha;
                    }
                    path = branch.CanonicalName.Split('/')[2];
                    if (commitChoosen != null)
                        break;
                }
            }
            if (commitChoosen != null)
            {
                allCommit.Add(commitChoosen);
                
                foreach (Commit commitParent in commitChoosen.Parents)
                {
                    getAllCommitBy(path, gitPath, commitParent.Sha);
                }
            }
        }

        private void addAllCommitByChoosen(string path, string gitPath, string shaChoosen)
        {
            repo = new LibGit2Sharp.Repository(gitPath);
            //List<CommitModel> commitListByValue = new List<CommitModel>();
            var p = path + "/";
            CommitModel commitValue = null;
            Commit commitChoosen = null;
            Branch branchChecker = repo.Branches.Where(bv => p.StartsWith(bv.Name + "/")).FirstOrDefault();
            Commit branchValue = branchChecker != null ? branchChecker.Tip : null;
            IEnumerable<Tag> tagValue = repo.Tags.Where(bv => p.StartsWith(bv.Name + "/"));

            if (branchValue != null)
                commitChoosen = repo.Branches[path].Commits.Where(c => c.Sha == shaChoosen).FirstOrDefault();
            else
            {
                List<Branch> allBranch = repo.Branches.ToList();
                foreach (Branch branch in allBranch)
                {
                    commitChoosen = branch.Commits.Where(c => c.Sha == tagValue.FirstOrDefault().Target.Sha).FirstOrDefault();
                    path = branch.CanonicalName.Split('/')[2];
                    if (commitChoosen != null)
                        break;
                }
            }
            if (commitChoosen != null)
            {
                commitValue = makeCommitFiller(commitChoosen, gitPath, path);
                commitListRecursif.Add(commitValue);
                foreach(Commit commitParent in commitChoosen.Parents)
                {
                    addAllCommitByChoosen(path, gitPath, commitParent.Sha);
                }
            }
        }

        public List<CommitModel> getAllCommitByChoosen(string path, string gitPath, string shaChoosen)
        {
            addAllCommitByChoosen(branchFinder(gitPath,path), gitPath, shaChoosen);

            return commitListRecursif;
        }

        public List<TagModel> getAllTag(string gitPath)
        {
            repo = new LibGit2Sharp.Repository(gitPath);
            IEnumerable<Tag> allTagValue = repo.Tags;
            List<TagModel> tagValue = new List<TagModel>();

            foreach (Tag tag in allTagValue)
            {
                tagValue.Add( new TagModel
                {
                    name = tag.Name,
                    message = tag.Annotation != null ? tag.Annotation.Message:"",
                    SHA = tag.Target.Sha,
                    when = tag.Annotation != null ? tag.Annotation.Tagger.When : ((Commit)tag.Target).Author.When
                });
            }

            return tagValue;
        }
        #endregion

        #region Filler

        private CommitModel makeCommitFiller(Commit commit, string gitPath, string branch)
        {
            CommitModel commitFiller = new CommitModel
            {
                Author = commit.Author,
                CommitCode = commit.Sha.Substring(0,6),
                Commiter = commit.Committer,
                CommitTime = commit.Committer.When.ToString(),
                GitPath = userGitPath(gitPath),
                CommitChange = getCommitChange(commit, gitPath),
                Parent = commit.Parents.Select(bp => bp.Sha).ToArray(),
                Message = commit.Message != null ? commit.Message : "",
                ShortMessage = commit.MessageShort != null ? commit.MessageShort : "",
                TreeEntry = getCommitDetail(commit, gitPath, branch),
                Feature = getFeature(commit.Message),
                SHA = commit.Sha
            };
            return commitFiller;
        }

        #endregion

        #region infoSplitter

        private string userGitPath(string path)
        {
            string[] urlSplitter = path.Split('\\');
            return urlSplitter[2] + "/" + urlSplitter[3];
        }

        private string getFeature(string message)
        {
            if (!message.Contains('#'))
                return "";

            string newString = "";

            if (message.Contains("\n"))
                newString = message.Replace("\n", "");

            string[] messagesplitter = newString.Split('#');
            if (messagesplitter.Count() > 1)
                return messagesplitter[1];
            else
                return messagesplitter[0];
        }

        #endregion
    }
}