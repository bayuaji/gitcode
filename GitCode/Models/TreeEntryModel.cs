using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibGit2Sharp;

namespace GitCode.Models
{
    public class TreeEntryModel
    {
        public Commit Commit { get; set; }
        public string Name { get; set; }
        public Mode Mode { get; set; }
        public string Path { get; set; }
        public TreeEntryTargetType TargetType { get; set; }
        public GitObject Target { get; set; }
    }
}