﻿using System;
using System.Collections.Generic;

namespace EOTools.Tools
{
    public class GitManager
    {
        private string WorkingDir = "";

        public GitManager(string _workingDir)
        {
            WorkingDir = _workingDir;
        }

        public void Stage(string _file)
        {
            string strCmdText;

            strCmdText = $"/C git stage \"{_file}\"";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                WorkingDirectory = WorkingDir,
                FileName = "CMD.exe",
                Arguments = strCmdText
            }).WaitForExit();
        }

        public void CommitAndPush(string _commitDesc)
        {
            string strCmdText;

            strCmdText = $"/C git commit -m \"{_commitDesc}\"";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                WorkingDirectory = WorkingDir,
                FileName = "CMD.exe",
                Arguments = strCmdText
            }).WaitForExit();

            strCmdText = $"/C git push";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                WorkingDirectory = WorkingDir,
                FileName = "CMD.exe",
                Arguments = strCmdText
            }).WaitForExit();

        }
    }
}
