using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorEmail
{
    [RunInstaller(true)]
    public partial class ReadEmals : System.Configuration.Install.Installer
    {
        public ReadEmals()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
