using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Reveal : Line {
    public override LineType GetLineType() {
        return LineType.Reveal;
    }
    public Reveal() {
    }
}