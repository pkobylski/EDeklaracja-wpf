using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core;

namespace E_Deklaracja_WPF.Services
{
    public class OpenDialogService : IOpenDialogService
    {
        public string OpenFileDialog(string defaultPath)
        {
            string result = string.Empty;

            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.InitialDirectory = defaultPath;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    result = dialog.FileName;
                }
            }

            return result;
        }
    }
}
