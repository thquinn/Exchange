using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Sacrifice : Line {
    public string card;
    public Line[] sublines;

    public override LineType GetLineType() {
        return LineType.Sacrifice;
    }
    public Sacrifice(string card, string[] lines) {
        this.card = card;
        List<Line> sublines = new List<Line>();
        foreach (string line in lines) {
            LineType lineType;
            string subline = Util.StripLineType(line, out lineType);
            if (lineType == LineType.Clear) {
                sublines.Add(new Clear());
            }
            else if (lineType == LineType.Get) {
                sublines.Add(new Get(subline));
            }
            else if (lineType == LineType.Go) {
                sublines.Add(new Go(subline));
            }
            else if (lineType == LineType.Lose) {
                sublines.Add(new Lose(subline));
            }
            else if (lineType == LineType.Reveal) {
                sublines.Add(new Reveal());
            }
            else if (lineType == LineType.Score) {
                sublines.Add(new Score(subline));
            }
            else if (lineType == LineType.Text) {
                sublines.Add(new Text(subline));
            }
            else if (lineType == LineType.Time) {
                sublines.Add(new Time(subline));
            }
        }
        this.sublines = sublines.ToArray();
    }
}