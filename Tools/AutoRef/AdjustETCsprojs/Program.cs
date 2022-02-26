// See https://aka.ms/new-console-template for more information
using System.Xml.Linq;
string command = args[0];
if (command == "refresh")
{
    string root = args[1].Trim();
    AdjustTool.Refresh(root + @"\Unity.Model.csproj", "Model");
    AdjustTool.Refresh(root + @"\Unity.ModelView.csproj", "ModelView");
    AdjustTool.Refresh(root + @"\Unity.Hotfix.csproj", "Hotfix");
    AdjustTool.Refresh(root + @"\Unity.HotfixView.csproj", "HotfixView");
}
else if (command == "clear")
{
    string root = args[1].Trim();
    AdjustTool.Refresh(root + @"\Unity.Model.csproj", "Model", false);
    AdjustTool.Refresh(root + @"\Unity.ModelView.csproj", "ModelView", false);
    AdjustTool.Refresh(root + @"\Unity.Hotfix.csproj", "Hotfix", false);
    AdjustTool.Refresh(root + @"\Unity.HotfixView.csproj", "HotfixView", false);
}
//string root = @"D:\Github\HoH\Unity";



public class AdjustTool
{
    public static void Refresh(string csprojPath, string asmFolderName, bool addComment = true)
    {
        string includeValue = $"Codes\\{asmFolderName}\\**\\*.cs";
        XDocument doc = XDocument.Load(csprojPath);
        XElement project = doc.Elements().First(e => e.Name.LocalName == "Project");
        var comments = doc.Nodes().Where(n => n.NodeType == System.Xml.XmlNodeType.Comment).ToList();
        foreach (var c in comments)
        {
            c.Remove();
        }
        var itemGroups = project.Elements().Where(e => e.Name.LocalName == "ItemGroup");
        List<XElement> delCompile = new List<XElement>();
        foreach (XElement itemGroup in itemGroups)
        {
            foreach (var element in itemGroup.Elements())
            {
                if (element.Name.LocalName == "Compile")
                {
                    delCompile.Add(element);
                }
            }
        }
        foreach (XElement item in delCompile)
        {
            item.Remove();
        }
        XElement firstItemGroup = project.Elements().First(e => e.Name.LocalName == "ItemGroup");
        XElement compile = new XElement(project.Name.ToString().Replace("Project", "Compile"), new XAttribute("Include", includeValue));
        firstItemGroup.Add(compile);
        if (addComment)
        {
            XComment xComment = new XComment(DateTime.Now.ToString());
            doc.Add(xComment);
        }
        doc.Save(csprojPath);
        Console.WriteLine($"刷新{asmFolderName}成功");
    }

    public static void Adjust(string csprojPath, string asmFolderName, string filePath, bool isAdd, bool addComment = true)
    {
        XDocument doc = XDocument.Load(csprojPath);
        XElement project = doc.Elements().First(e => e.Name.LocalName == "Project");
        XElement firstItemGroup = project.Elements().First(e => e.Name.LocalName == "ItemGroup");
        XElement compileAll = new XElement(project.Name.ToString().Replace("Project", "Compile"), new XAttribute("Include", $"Codes\\{asmFolderName}\\**\\*.cs"));
        if (firstItemGroup.Elements().FirstOrDefault(e => e.FirstAttribute?.ToString() == compileAll.FirstAttribute?.ToString()) != default(XElement))
        {
            return;
        }

        string includeValue = filePath[filePath.IndexOf("Codes")..];
        List<XNode>? comments = doc.Nodes().Where(n => n.NodeType == System.Xml.XmlNodeType.Comment).ToList();
        foreach (XNode? c in comments)
        {
            c.Remove();
        }
        XElement changedCompileNode = new(project.Name.ToString().Replace("Project", "Compile"), new XAttribute("Include", includeValue));
        if (isAdd)
        {
            if (firstItemGroup.Elements().FirstOrDefault(e => e.FirstAttribute?.ToString() == changedCompileNode.FirstAttribute?.ToString()) == default(XElement))
            {
                XElement lastCompileNode = firstItemGroup.Elements().First();
                foreach (XElement? compileNode in firstItemGroup.Elements())
                {
                    if (compileNode.ToString().StartsWith("<Compile"))
                    {
                        lastCompileNode = compileNode;
                    }
                    else
                    {
                        break;
                    }
                }
                lastCompileNode.AddAfterSelf(changedCompileNode); 
            }
        }
        else
        {
            XElement? removeNode = firstItemGroup.Elements().FirstOrDefault(e => e.FirstAttribute?.ToString() == changedCompileNode.FirstAttribute?.ToString());
            if (removeNode != default(XElement))
            {
                removeNode.Remove();
            }
        }
        if (addComment)
        {
            XComment xComment = new XComment(DateTime.Now.ToString());
            doc.Add(xComment);
        }
        doc.Save(csprojPath);
        Console.WriteLine($"刷新{asmFolderName}成功");
    }
}