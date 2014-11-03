using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibGit2Sharp;

namespace GitCode.Models
{
    public class RepositoryInfoModel
    {
        public string RepoName { get; set; }
        public string GitPath { get; set; }
        public IEnumerable<TreeEntryModel> TreeEntry { get; set; }

    }
}