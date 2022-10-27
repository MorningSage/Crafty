using System.Collections.Generic;
using System.Text.RegularExpressions;

public struct LinkItem
{
    public string Href;
    public string Text;

    public override string ToString()
    {
        return Href + "\n\t" + Text;
    }
}

static class LinkFinder
{
    public static List<LinkItem> Find(string file)
    {
        List<LinkItem> list = new List<LinkItem>();

        MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

        foreach (Match m in m1)
        {
            string value = m.Groups[1].Value;
            LinkItem i = new LinkItem();

            Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
            if (m2.Success)
            {
                i.Href = m2.Groups[1].Value;
            }

            string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
            i.Text = t;

            list.Add(i);
        }
        return list;
    }
}