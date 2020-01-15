using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorEmail.Models
{
    public class Anexo
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileByte { get; set; }
    }
}
