using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Score : Line {
    public int points;

    public override LineType GetLineType() {
        return LineType.Score;
    }
    public Score(string line) {
        points = int.Parse(line);
    }
}