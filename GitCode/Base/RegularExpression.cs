﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GitCode.Base
{
    public static class RegularExpression
    {
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string Username = @"^[a-zA-Z][a-zA-Z0-9\-_]+$";
        public const string Teamname = @"^[a-zA-Z][a-zA-Z0-9\-_]+$";

        public static readonly Regex ReplaceNewline = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
    }
}