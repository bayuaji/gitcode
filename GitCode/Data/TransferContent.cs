using GitCode.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitCode.Data
{
    public class TransferContent
    {
        private GitCodeContext db = new GitCodeContext();

        public string repoDirectory;
        public string getRepoDirectory()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().RepositoryPath;
            return repoDirectory;
        }

        public string getGitDirectory()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().GitPath;
            return repoDirectory;
        }

        public string getCacheDirectory()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().CachePath;
            return repoDirectory;
        }



        public string getAbout()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().AboutContent;
            return repoDirectory;
        }

        public string getCommit()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().CommitContent;
            return repoDirectory;
        }

        public string getTeam()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().TeamContent;
            return repoDirectory;
        }

        public string getClass()
        {
            repoDirectory = db.GitCodeBasses.FirstOrDefault().ClassContent;
            return repoDirectory;
        }
        public void setRepoDirectory()
        {
        }
    }
}