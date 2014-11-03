using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibGit2Sharp;

namespace GitCode.Models
{
    public class CommitModel
    {
        public string CommitCode { get; set; }
        public Signature Author { get; set; }
        public Signature Commiter { get; set; }
        public string Message { get; set; }
        public string ShortMessage { get; set; }
        public string CommitTime { get; set; }
        public string SHA { get; set; }
        public IEnumerable<CommitChangeModel> CommitChange { get; set; }
        public IEnumerable<TreeEntryModel> TreeEntry { get; set; }
        public string[] Parent { get; set; }
        public string GitPath { get; set; }
        public string Feature { get; set; }
    }
}