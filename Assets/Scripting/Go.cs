using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Go : Line {
    public string page;

    public override LineType GetLineType() {
        return LineType.Go;
    }
    public Go(string line) {
        page = line;
    }
}