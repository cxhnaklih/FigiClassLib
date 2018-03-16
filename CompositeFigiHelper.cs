using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Naklih.Com.FigiClassLib
{
    public class CompositeFigiHelper
    {
        public Dictionary<string,int> _composites = new Dictionary<string, int>();

        private static CompositeFigiHelper _instance;

        public static CompositeFigiHelper Instance
        {
            get
            {
                if(_instance== null)
                {
                    _instance = new CompositeFigiHelper();
                }
                return _instance;
            }
        }

        protected CompositeFigiHelper()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader tr = new StreamReader(assembly.GetManifestResourceStream("Naklih.Com.FigiClassLib.Composites.tsv"));
            tr.ReadLine(); // skip header...
            int i = 0;
            while (tr.EndOfStream == false)
            {
                i++; 
                string data = tr.ReadLine();
                string[] kvp = data.Split('\t');
                if(kvp.Length ==2)
                {
                    _composites.Add(kvp[0], i);
                }
                
            }
            
        }

        public bool IsComposite(string Exchange)
        {
            if(Exchange is null)
            {
                return false;
            }
            return (_composites.ContainsKey(Exchange));
        }

        public int CompositePriority(string Exchange)
        {
            if (Exchange is null)
            {
                return 9999;
            }

            return (_composites[Exchange]);
        }
    }
}
