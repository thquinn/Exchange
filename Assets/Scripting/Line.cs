using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

abstract class Line {
    public abstract LineType GetLineType();
}

public enum LineType {
    Clear, Get, Go, Lose, Reveal, Sacrifice, Score, TBC, Text, Time
}