using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Lose : Line {
    string card;

    public override LineType GetLineType() {
        return LineType.Lose;
    }
    public Lose(string line) {
        card = line;
    }
}