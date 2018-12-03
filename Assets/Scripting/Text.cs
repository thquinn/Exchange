using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Text : Line {
    public string text;

    public override LineType GetLineType() {
        return LineType.Text;
    }
    public Text(string line) {
        text = line.Replace(@"\n", "\n");
    }
}