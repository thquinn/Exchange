using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Get : Line {
    public string card;

    public override LineType GetLineType() {
        return LineType.Get;
    }
    public Get(string line) {
        card = line;
    }
}