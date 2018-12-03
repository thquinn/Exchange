using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Time : Line {
    public int day;
    public string time;

    public override LineType GetLineType() {
        return LineType.Time;
    }
    public Time(string line) {
        string[] tokens = line.Split(' ');
        day = int.Parse(tokens[0]);
        time = line.Substring(line.IndexOf(' ') + 1);
    }
}