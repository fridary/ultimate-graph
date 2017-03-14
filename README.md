Ultimate Graph
====

![](https://raw.githubusercontent.com/fridary/ultimate-graph/master/screen.png)

Ultimate graph is a program for creating mathematics graph and real-time visualizing widely popular algorithms like Breadth-first Search, Depth-first Search, Bridge Search and Dijkstra’s algorithm (Shortest Path First) written on C#.

You can start timer for animating algorithms, explaining step-by-step how it works each period of time (3 seconds as default).



Custom plugins
----------

![](https://raw.githubusercontent.com/fridary/ultimate-graph/master/debugger.jpg)

Another Ultimate Graph feature is possibility of writing your own algorithms with `System.CodeDom.Compiler`. The code is compiled into .dll plugin that will be automatically included in program.

Let's try code plugin that paints all vertices in random color one by one:
```c#
int i = 0;
private void timer_Tick(object sender, EventArgs e) {
    if (i != dot.Count)
    {
        Random rd = new Random();
        dot[i].highlight = true;
        dot[i].highlight_color = Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
        this.label1.Text = "Drawing " + (i + 1) + " vertice";
        i++;
    }
    else
    {
        control_stripStatusText = "Done";
        this.label.Text = "Done";
        timer.Stop(); // stop timer when painted last vertice
    }

    UpdatePlugin(this, e);
}
```



Class diagram
----------

![](https://raw.githubusercontent.com/fridary/ultimate-graph/master/classes.jpg)
