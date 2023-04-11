namespace PowershellPlugin.Predictor
{
    public static class LoadTree
    {
        public static void Load(this PreffixTree tree, string path)
        {
            var files = System.IO.Directory.GetFiles(path);
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
