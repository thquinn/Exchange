using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Clear : Line {
    public override LineType GetLineType() {
        return LineType.Clear;
    }
    public Clear() {
    }
}