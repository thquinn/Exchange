using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TBC : Line {
    public override LineType GetLineType() {
        return LineType.TBC;
    }
    public TBC() {
    }
}