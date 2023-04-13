namespace PowershellPlugin.Predictor
{
    public static class LoadTree
    {
        public static void Load(this PreffixTree tree, string path, string pattern)
        {
            var files = System.IO.Directory.GetFiles(path, pattern);
            foreach (var file in files)
            {
                var lines = System.IO.File.ReadAllLines(file);
                foreach(var line in lines)
                {
                    tree.Insert(line);
                }
            }
        }
    }
}
