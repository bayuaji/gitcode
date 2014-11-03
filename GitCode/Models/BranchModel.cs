using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibGit2Sharp;

namespace GitCode.Models
{
    public class BranchModel
    {
        public string BranchName { get; set; }
        public Commit CommitInBranch { get; set; }
    }
}