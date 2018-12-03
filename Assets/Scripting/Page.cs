using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Page {
    public Line[] lines;

    public Page(string[] strings) {
        List<Line> lines = new List<Line>();
        string currentSacrifice = "";
        List<string> currentSacrificeLines = new List<string>();
        foreach (string s in strings) {
            string trimmed = s.Trim();
            if (trimmed.Length == 0) {
                continue;
            }

            // Finalizing sacrifice sublines.
            if (currentSacrifice != "") {
                if (s.StartsWith("\t")) {
                    currentSacrificeLines.Add(trimmed);
                    continue;
                }
                else {
                    lines.Add(new Sacrifice(currentSacrifice, currentSacrificeLines.ToArray()));
                    currentSacrifice = "";
                }
            }

            LineType lineType;
            string stripped = Util.StripLineType(trimmed, out lineType);
            if (lineType == LineType.Clear) {
                lines.Add(new Clear());
            }
            else if (lineType == LineType.Get) {
                lines.Add(new Get(stripped));
            }
            else if (lineType == LineType.Go) {
                lines.Add(new Go(stripped));
            }
            else if (lineType == LineType.Lose) {
                lines.Add(new Lose(stripped));
            }
            else if (lineType == LineType.Reveal) {
                lines.Add(new Reveal());
            }
            else if (lineType == LineType.Sacrifice) {
                currentSacrifice = stripped;
                currentSacrificeLines.Clear();
            }
            else if (lineType == LineType.Score) {
                lines.Add(new Score(stripped));
            }
            else if (lineType == LineType.Text) {
                lines.Add(new Text(stripped));
            }
            else if (lineType == LineType.Time) {
                lines.Add(new Time(stripped));
            }
        }
        if (currentSacrifice != "") {
            lines.Add(new Sacrifice(currentSacrifice, currentSacrificeLines.ToArray()));
        }
        this.lines = lines.ToArray();
    }
}