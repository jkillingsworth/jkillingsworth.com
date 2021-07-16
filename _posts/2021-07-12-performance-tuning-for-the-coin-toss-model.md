---
layout: post
title: Performance Tuning for the Coin Toss Model
---

I wrapped up the [last post]({% post_url 2021-05-31-approximations-with-polynomials %}) expressing a desire to study the approximation technique using larger models of the coin toss game. Up until now, I was using a naive implementation of the computation method to perform the calculations---an implementation that was crudely implemented and too slow for larger models. In this post, I demonstrate an alternative approach that has a much better performance profile. I also describe a simple technique that can be used to reduce the number of iterations required when applying the hill climbing algorithm.

<!--excerpt-->

## Optimized Computation Method

To get around the performance issues referenced above, I decided to implement the computation method using an entirely different approach. I think the best way to describe this new approach is to work through an example. Suppose we have a model of the coin toss game with four coin toss events per round:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    n = 4
    \end{displaymath}
    \end{document}
{% endlatex %}

Just like we did in some of the previous posts, we can create a graphical representation of the coin toss model using a state diagram:

{% latex fig-02 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,automata}
    \begin{document}
    \begin{tikzpicture}[auto,>=stealth',shorten >=1bp,node distance=1.125in]
    \tikzset{every state/.style={minimum size=0.5in}}
    \node[state,initial right] (00)               {$S_0$};
    \node[state]               (+1) [above of=00] {$S_{+1}$};
    \node[state]               (+2) [right of=+1] {$S_{+2}$};
    \node[state]               (+3) [right of=+2] {$S_{+3}$};
    \node[state]               (+4) [right of=+3] {$S_{+4}$};
    \node[state]               (-1) [below of=00] {$S_{-1}$};
    \node[state]               (-2) [right of=-1] {$S_{-2}$};
    \node[state]               (-3) [right of=-2] {$S_{-3}$};
    \node[state]               (-4) [right of=-3] {$S_{-4}$};
    \path[->]
    (00) edge              node [swap] {$p_0$} (+1)
    (+1) edge [bend right] node [swap] {$q_1$} (00)
    (+1) edge [bend left]  node        {$p_1$} (+2)
    (+2) edge [bend left]  node        {$q_2$} (+1)
    (+2) edge [bend left]  node        {$p_2$} (+3)
    (+3) edge [bend left]  node        {$q_3$} (+2)
    (+3) edge              node        {$p_3$} (+4)
    (00) edge              node        {$p_0$} (-1)
    (-1) edge [bend left]  node        {$q_1$} (00)
    (-1) edge [bend left]  node        {$p_1$} (-2)
    (-2) edge [bend left]  node        {$q_2$} (-1)
    (-2) edge [bend left]  node        {$p_2$} (-3)
    (-3) edge [bend left]  node        {$q_3$} (-2)
    (-3) edge              node        {$p_3$} (-4);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

This diagram illustrates the starting state and all of the possible state transitions. In this case, we use one set of variables to represent state transitions away from the initial state and another set of variables to represent state transitions towards the initial state. Here is the relationship between these two sets of variables:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    q_i = 1 - p_i, \quad \forall i \in \{\, 0, \dots, n - 1 \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to precompute these values ahead of time and look them up later instead of computing them on the fly. Since our model is symmetrical by definition, we can assume the coin in the initial state is always a fair coin:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_0 & = 0.5000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

For this example, let's choose some arbitrary values for the remaining state transitions:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.4000
    \\
    p_2 & = 0.3000
    \\
    p_3 & = 0.2000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now we want to create some lookup tables for our state transition values. We'll use these lookup tables when computing the likelihood of landing on each one of the states after each toss of the coin. Let's create two arrays and populate them with these values:

{% latex fig-06 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{frame/.style={draw=black}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{p}$};
    \node[cell={frame,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={frame,inner,value}] (vp0)   at ( 0,   1.0) {0.5000};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {0.4000};
    \node[cell={frame,inner,value}]         at ( 2,   1.0) {0.3000};
    \node[cell={frame,inner,value}]         at ( 3,   1.0) {0.2000};
    \node[cell={frame,inner,empty}]         at ( 4,   1.0) {0};
    \node[cell={frame,outer,empty}]         at ( 5,   1.0) {0};
    \node[pnode]                    (pp0)   at ( 0,   2.7) {$p_{0}$};
    \path[ppath]                    (pp0)   -- (vp0);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{q}$};
    \node[cell={frame,outer,empty}]         at (-1,   4.5) {0};
    \node[cell={frame,inner,value}] (vq0)   at ( 0,   4.5) {0.5000};
    \node[cell={frame,inner,value}]         at ( 1,   4.5) {0.6000};
    \node[cell={frame,inner,value}]         at ( 2,   4.5) {0.7000};
    \node[cell={frame,inner,value}]         at ( 3,   4.5) {0.8000};
    \node[cell={frame,inner,empty}]         at ( 4,   4.5) {0};
    \node[cell={frame,outer,empty}]         at ( 5,   4.5) {0};
    \node[pnode]                    (pq0)   at ( 0,   6.2) {$q_{0}$};
    \path[ppath]                    (pq0)   -- (vq0);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

These are our lookup tables. Notice that these arrays are padded with three extra elements, one in the front and two in the back. You'll see in a minute why this is necessary. We are using pointers to refer to the first value in each array. Now let's do some pointer arithmetic:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \begin{drcases}
    \mathrlap{p^{\sscr{heads}}_{i}}\phantom{p^{\sscr{heads}}} = p_{(i - 1)}\quad
    \\[0.5em]
    \mathrlap{q^{\sscr{tails}}_{i}}\phantom{p^{\sscr{heads}}} = q_{(i + 1)}\quad
    \end{drcases}
    \quad \forall i \in \{\, 0, \dots, n \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

One pointer is incremented, while the other is decremented. This effectively shifts these arrays, one to the right and one to the left. We want to align them in a way that makes it easy to perform our computations later on. Here is how our lookup tables appear after the shift:

{% latex fig-08 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{frame/.style={draw=black}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \tikzset{edge/.style args={#1,#2,#3}{minimum width=0.125in,#2,append after command={
        \pgfextra
        \begin{pgfinterruptpath}
        \begin{pgfonlayer}{foreground}
        \path[#1]
        let
        \p1 = ($(\tikzlastnode.north west)+(+0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p2 = ($(\tikzlastnode.north east)+(-0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p3 = ($(\tikzlastnode.south west)+(+0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p4 = ($(\tikzlastnode.south east)+(-0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p5 = ($(\tikzlastnode.south east)$),
        in
        (\p1) -- (\p2)
        (\p3) -- (\p4)
        \if#3L (\p2) -- (\p4) \fi
        \if#3R (\p1) -- (\p3) \fi
        ;
        \end{pgfonlayer}
        \end{pgfinterruptpath}
        \endpgfextra
    }}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{p}$};
    \node[cell={frame,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {0.5000};
    \node[cell={frame,inner,value}]         at ( 2,   1.0) {0.4000};
    \node[cell={frame,inner,value}]         at ( 3,   1.0) {0.3000};
    \node[cell={frame,inner,value}]         at ( 4,   1.0) {0.2000};
    \node[cell={frame,outer,empty}]         at ( 5,   1.0) {0};
    \node[edge={frame,outer,R}]             at ( 5.6, 1.0) {};
    \node[pnode]                    (pph0)  at ( 0,   2.7) {$p^{\sscr{heads}}_{0}$};
    \path[ppath]                    (pph0)  -- (vph0);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{q}$};
    \node[edge={frame,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={frame,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={frame,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={frame,inner,value}]         at ( 1,   4.5) {0.7000};
    \node[cell={frame,inner,value}]         at ( 2,   4.5) {0.8000};
    \node[cell={frame,inner,empty}]         at ( 3,   4.5) {0};
    \node[cell={frame,inner,empty}]         at ( 4,   4.5) {0};
    \node[pnode]                    (pqt0)  at ( 0,   6.2) {$q^{\sscr{tails}}_{0}$};
    \path[ppath]                    (pqt0)  -- (vqt0);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

By definition, every round of the coin toss game starts out in the zero state, so we know with 100% certainty what state we're going to be in before the first coin toss. Thus, we can represent our initial state vector with the following array:

{% latex fig-09 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{frame/.style={draw=black}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{r}_{0}$};
    \node[cell={frame,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={frame,inner,value}] (vr00)  at ( 0,   1.0) {1.0000};
    \node[cell={frame,inner,empty}]         at ( 1,   1.0) {0};
    \node[cell={frame,inner,empty}]         at ( 2,   1.0) {0};
    \node[cell={frame,inner,empty}]         at ( 3,   1.0) {0};
    \node[cell={frame,inner,empty}]         at ( 4,   1.0) {0};
    \node[cell={frame,outer,empty}]         at ( 5,   1.0) {0};
    \node[pnode]                    (pr00)  at ( 0,   2.7)   {$r_{0,0}$};
    \path[ppath]                    (pr00)  -- (vr00);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

This array is allocated to the same size as the arrays used for the state transition lookup tables. And like before, we use a pointer to refer to the first value in the array. Now let's do some more pointer arithmetic:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \begin{drcases}
    \mathrlap{r^{\sscr{heads}}_{k,i}}\phantom{r^{\sscr{heads}}} = r_{k,(i - 1)}\quad
    \\[0.5em]
    \mathrlap{r^{\sscr{tails}}_{k,i}}\phantom{r^{\sscr{heads}}} = r_{k,(i + 1)}\quad
    \end{drcases}
    \quad \forall i \in \{\, 0, \dots, n \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

In this case, we create a pair of pointers that point to two different elements of the same array. We can treat these two pointers as if they were pointers to two different arrays, even though they're really not. In essence, this is what our two arrays look like:

{% latex fig-11 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{frame/.style={draw=black}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \tikzset{edge/.style args={#1,#2,#3}{minimum width=0.125in,#2,append after command={
        \pgfextra
        \begin{pgfinterruptpath}
        \begin{pgfonlayer}{foreground}
        \path[#1]
        let
        \p1 = ($(\tikzlastnode.north west)+(+0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p2 = ($(\tikzlastnode.north east)+(-0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p3 = ($(\tikzlastnode.south west)+(+0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p4 = ($(\tikzlastnode.south east)+(-0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p5 = ($(\tikzlastnode.south east)$),
        in
        (\p1) -- (\p2)
        (\p3) -- (\p4)
        \if#3L (\p2) -- (\p4) \fi
        \if#3R (\p1) -- (\p3) \fi
        ;
        \end{pgfonlayer}
        \end{pgfinterruptpath}
        \endpgfextra
    }}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{r}_{0}$};
    \node[cell={frame,inner,empty}] (vrh00) at ( 0,   1.0) {0};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {1.0000};
    \node[cell={frame,inner,empty}]         at ( 2,   1.0) {0};
    \node[cell={frame,inner,empty}]         at ( 3,   1.0) {0};
    \node[cell={frame,inner,empty}]         at ( 4,   1.0) {0};
    \node[cell={frame,outer,empty}]         at ( 5,   1.0) {0};
    \node[edge={frame,outer,R}]             at ( 5.6, 1.0) {};
    \node[pnode]                    (prh00) at ( 0,   2.7) {$r^{\sscr{heads}}_{0,0}$};
    \path[ppath]                    (prh00) -- (vrh00);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{r}_{0}$};
    \node[edge={frame,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={frame,outer,value}]         at (-1,   4.5) {1.0000};
    \node[cell={frame,inner,empty}] (vrt00) at ( 0,   4.5) {0};
    \node[cell={frame,inner,empty}]         at ( 1,   4.5) {0};
    \node[cell={frame,inner,empty}]         at ( 2,   4.5) {0};
    \node[cell={frame,inner,empty}]         at ( 3,   4.5) {0};
    \node[cell={frame,inner,empty}]         at ( 4,   4.5) {0};
    \node[pnode]                    (prt00) at ( 0,   6.2) {$r^{\sscr{tails}}_{0,0}$};
    \path[ppath]                    (prt00) -- (vrt00);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Now that we have arrays representing our state transition values and our initial state vector, along with pointers that properly align the data, we can compute the values of the state vector after each toss of the coin. Here is the algorithm:

{% latex fig-12 %}
    \usepackage[vlined]{algorithm2e}
    \begin{document}
    \begin{algorithm}[H]
    \DontPrintSemicolon
    \For{$k = 1$ \KwTo $n$}{
        \BlankLine
        $j = k - 1$\;
        \BlankLine
        \For{$i = 0$ \KwTo $k$}{
            \BlankLine
            $
            r_{k,i}
            \leftarrow
            \Big( p^{\sscr{heads}}_{i} \cdot r^{\sscr{heads}}_{j,i} \Big)
            +
            \Big( q^{\sscr{tails}}_{i} \cdot r^{\sscr{tails}}_{j,i} \Big)
            $\;
        }
        \BlankLine
        $r_{k,0} \leftarrow 2 \cdot r_{k,0}$\;
    }
    \end{algorithm}
    \end{document}
{% endlatex %}

Notice that we always double the value in the zero offset at the end of each iteration of the outer loop. This is necessary because we are only solving half the problem. Since we know our model is symmetrical, we don't bother to calculate values for transitions into the negative states. They are always a mirror image of the values for the positive states. However, we do need to consider the negative state that transitions into the zero state. It is the same as the positive state that transitions into the zero state, hence the doubling.

Here are the computed values after outer loop iteration #1:

{% latex fig-13 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{major/.style={draw=black}}
    \tikzset{minor/.style={draw=gray}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{write/.style={text=red}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \tikzset{edge/.style args={#1,#2,#3}{minimum width=0.125in,#2,append after command={
        \pgfextra
        \begin{pgfinterruptpath}
        \begin{pgfonlayer}{foreground}
        \path[#1]
        let
        \p1 = ($(\tikzlastnode.north west)+(+0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p2 = ($(\tikzlastnode.north east)+(-0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p3 = ($(\tikzlastnode.south west)+(+0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p4 = ($(\tikzlastnode.south east)+(-0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p5 = ($(\tikzlastnode.south east)$),
        in
        (\p1) -- (\p2)
        (\p3) -- (\p4)
        \if#3L (\p2) -- (\p4) \fi
        \if#3R (\p1) -- (\p3) \fi
        ;
        \end{pgfonlayer}
        \end{pgfinterruptpath}
        \endpgfextra
    }}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{p}$};
    \node[cell={minor,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.4000};
    \node[cell={minor,inner,value}]         at ( 3,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 4,   1.0) {0.2000};
    \node[cell={minor,outer,empty}]         at ( 5,   1.0) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 1.0) {};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{0}$};
    \node[cell={minor,inner,empty}] (vrh00) at ( 0,   2.5) {0};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {1.0000};
    \node[cell={minor,inner,empty}]         at ( 2,   2.5) {0};
    \node[cell={minor,inner,empty}]         at ( 3,   2.5) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   2.5) {0};
    \node[cell={minor,outer,empty}]         at ( 5,   2.5) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 2.5) {};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{q}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 2,   4.5) {0.8000};
    \node[cell={minor,inner,empty}]         at ( 3,   4.5) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{0}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {1.0000};
    \node[cell={minor,inner,empty}] (vrt00) at ( 0,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 1,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 3,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{1}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr10)  at ( 0,   8.0) {0.0000};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.5000};
    \node[cell={major,inner,empty}]         at ( 2,   8.0) {0};
    \node[cell={major,inner,empty}]         at ( 3,   8.0) {0};
    \node[cell={major,inner,empty}]         at ( 4,   8.0) {0};
    \node[cell={major,outer,empty}]         at ( 5,   8.0) {0};
    \node[pnode]                    (pr10)  at ( 0,   9.7) {$r_{1,0}$};
    \path[ppath]                    (pr10)  -- (vr10);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #2:

{% latex fig-14 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{major/.style={draw=black}}
    \tikzset{minor/.style={draw=gray}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{write/.style={text=red}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \tikzset{edge/.style args={#1,#2,#3}{minimum width=0.125in,#2,append after command={
        \pgfextra
        \begin{pgfinterruptpath}
        \begin{pgfonlayer}{foreground}
        \path[#1]
        let
        \p1 = ($(\tikzlastnode.north west)+(+0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p2 = ($(\tikzlastnode.north east)+(-0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p3 = ($(\tikzlastnode.south west)+(+0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p4 = ($(\tikzlastnode.south east)+(-0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p5 = ($(\tikzlastnode.south east)$),
        in
        (\p1) -- (\p2)
        (\p3) -- (\p4)
        \if#3L (\p2) -- (\p4) \fi
        \if#3R (\p1) -- (\p3) \fi
        ;
        \end{pgfonlayer}
        \end{pgfinterruptpath}
        \endpgfextra
    }}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{p}$};
    \node[cell={minor,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.4000};
    \node[cell={minor,inner,value}]         at ( 3,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 4,   1.0) {0.2000};
    \node[cell={minor,outer,empty}]         at ( 5,   1.0) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 1.0) {};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{1}$};
    \node[cell={minor,inner,empty}] (vrh10) at ( 0,   2.5) {0};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.0000};
    \node[cell={minor,inner,value}]         at ( 2,   2.5) {0.5000};
    \node[cell={minor,inner,empty}]         at ( 3,   2.5) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   2.5) {0};
    \node[cell={minor,outer,empty}]         at ( 5,   2.5) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 2.5) {};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{q}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 2,   4.5) {0.8000};
    \node[cell={minor,inner,empty}]         at ( 3,   4.5) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{1}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {0.0000};
    \node[cell={minor,inner,value}] (vrt10) at ( 0,   6.0) {0.5000};
    \node[cell={minor,inner,empty}]         at ( 1,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 3,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{2}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr20)  at ( 0,   8.0) {0.6000};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.0000};
    \node[cell={major,inner,write}]         at ( 2,   8.0) {0.2000};
    \node[cell={major,inner,empty}]         at ( 3,   8.0) {0};
    \node[cell={major,inner,empty}]         at ( 4,   8.0) {0};
    \node[cell={major,outer,empty}]         at ( 5,   8.0) {0};
    \node[pnode]                    (pr20)  at ( 0,   9.7) {$r_{2,0}$};
    \path[ppath]                    (pr20)  -- (vr20);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #3:

{% latex fig-15 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{major/.style={draw=black}}
    \tikzset{minor/.style={draw=gray}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{write/.style={text=red}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \tikzset{edge/.style args={#1,#2,#3}{minimum width=0.125in,#2,append after command={
        \pgfextra
        \begin{pgfinterruptpath}
        \begin{pgfonlayer}{foreground}
        \path[#1]
        let
        \p1 = ($(\tikzlastnode.north west)+(+0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p2 = ($(\tikzlastnode.north east)+(-0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p3 = ($(\tikzlastnode.south west)+(+0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p4 = ($(\tikzlastnode.south east)+(-0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p5 = ($(\tikzlastnode.south east)$),
        in
        (\p1) -- (\p2)
        (\p3) -- (\p4)
        \if#3L (\p2) -- (\p4) \fi
        \if#3R (\p1) -- (\p3) \fi
        ;
        \end{pgfonlayer}
        \end{pgfinterruptpath}
        \endpgfextra
    }}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{p}$};
    \node[cell={minor,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.4000};
    \node[cell={minor,inner,value}]         at ( 3,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 4,   1.0) {0.2000};
    \node[cell={minor,outer,empty}]         at ( 5,   1.0) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 1.0) {};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{2}$};
    \node[cell={minor,inner,empty}] (vrh20) at ( 0,   2.5) {0};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 2,   2.5) {0.0000};
    \node[cell={minor,inner,value}]         at ( 3,   2.5) {0.2000};
    \node[cell={minor,inner,empty}]         at ( 4,   2.5) {0};
    \node[cell={minor,outer,empty}]         at ( 5,   2.5) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 2.5) {};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{q}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 2,   4.5) {0.8000};
    \node[cell={minor,inner,empty}]         at ( 3,   4.5) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{2}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {0.6000};
    \node[cell={minor,inner,value}] (vrt20) at ( 0,   6.0) {0.0000};
    \node[cell={minor,inner,value}]         at ( 1,   6.0) {0.2000};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 3,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{3}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr30)  at ( 0,   8.0) {0.0000};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.4400};
    \node[cell={major,inner,write}]         at ( 2,   8.0) {0.0000};
    \node[cell={major,inner,write}]         at ( 3,   8.0) {0.0600};
    \node[cell={major,inner,empty}]         at ( 4,   8.0) {0};
    \node[cell={major,outer,empty}]         at ( 5,   8.0) {0};
    \node[pnode]                    (pr30)  at ( 0,   9.7) {$r_{3,0}$};
    \path[ppath]                    (pr30)  -- (vr30);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #4:

{% latex fig-16 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,calc}
    \begin{document}
    \begin{tikzpicture}[x=+0.625in,y=-0.25in]
    \pgfdeclarelayer{foreground}
    \pgfsetlayers{main,foreground}
    \tikzset{every node/.style={text height=10bp,text depth=3.5bp,inner sep=2.25bp}}
    \tikzset{index/.style={text=gray}}
    \tikzset{array/.style={text width=10bp,align=left}}
    \tikzset{pnode/.style={text width=10bp,align=left}}
    \tikzset{ppath/.style={draw,-stealth',shorten >=1bp}}
    \tikzset{major/.style={draw=black}}
    \tikzset{minor/.style={draw=gray}}
    \tikzset{inner/.style={}}
    \tikzset{outer/.style={fill=black!10}}
    \tikzset{value/.style={text=black}}
    \tikzset{empty/.style={text=gray}}
    \tikzset{write/.style={text=red}}
    \tikzset{cell/.style args={#1,#2,#3}{minimum width=0.625in,#1,#2,#3}}
    \tikzset{edge/.style args={#1,#2,#3}{minimum width=0.125in,#2,append after command={
        \pgfextra
        \begin{pgfinterruptpath}
        \begin{pgfonlayer}{foreground}
        \path[#1]
        let
        \p1 = ($(\tikzlastnode.north west)+(+0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p2 = ($(\tikzlastnode.north east)+(-0.5\pgflinewidth,-0.5\pgflinewidth)$),
        \p3 = ($(\tikzlastnode.south west)+(+0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p4 = ($(\tikzlastnode.south east)+(-0.5\pgflinewidth,+0.5\pgflinewidth)$),
        \p5 = ($(\tikzlastnode.south east)$),
        in
        (\p1) -- (\p2)
        (\p3) -- (\p4)
        \if#3L (\p2) -- (\p4) \fi
        \if#3R (\p1) -- (\p3) \fi
        ;
        \end{pgfonlayer}
        \end{pgfinterruptpath}
        \endpgfextra
    }}}
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[index]                            at ( 4,  -0.6) {4};
    \node[index]                            at ( 5,  -0.6) {5};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{p}$};
    \node[cell={minor,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.4000};
    \node[cell={minor,inner,value}]         at ( 3,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 4,   1.0) {0.2000};
    \node[cell={minor,outer,empty}]         at ( 5,   1.0) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 1.0) {};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{3}$};
    \node[cell={minor,inner,empty}] (vrh30) at ( 0,   2.5) {0};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.0000};
    \node[cell={minor,inner,value}]         at ( 2,   2.5) {0.4400};
    \node[cell={minor,inner,value}]         at ( 3,   2.5) {0.0000};
    \node[cell={minor,inner,value}]         at ( 4,   2.5) {0.0600};
    \node[cell={minor,outer,empty}]         at ( 5,   2.5) {0};
    \node[edge={minor,outer,R}]             at ( 5.6, 2.5) {};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{q}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 2,   4.5) {0.8000};
    \node[cell={minor,inner,empty}]         at ( 3,   4.5) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{3}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {0.0000};
    \node[cell={minor,inner,value}] (vrt30) at ( 0,   6.0) {0.4400};
    \node[cell={minor,inner,value}]         at ( 1,   6.0) {0.0000};
    \node[cell={minor,inner,value}]         at ( 2,   6.0) {0.0600};
    \node[cell={minor,inner,empty}]         at ( 3,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 4,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{4}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr40)  at ( 0,   8.0) {0.5280};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.0000};
    \node[cell={major,inner,write}]         at ( 2,   8.0) {0.2240};
    \node[cell={major,inner,write}]         at ( 3,   8.0) {0.0000};
    \node[cell={major,inner,write}]         at ( 4,   8.0) {0.0120};
    \node[cell={major,outer,empty}]         at ( 5,   8.0) {0};
    \node[pnode]                    (pr40)  at ( 0,   9.7) {$r_{4,0}$};
    \path[ppath]                    (pr40)  -- (vr40);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Once the loop terminates, we have the probabilities of landing on each state after the fourth and final toss of the coin. States with a zero value are never terminal states. The states with non-zero values are the terminal states. Here are the relevant values:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.5280
    \\
    r_2 & = 0.2240
    \\
    r_4 & = 0.0120
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Is this the most optimal computation method? Probably not. For even numbered coin tosses, the odd numbered states are always zero. For odd numbered coin tosses, the even numbered states are always zero. We could probably use this knowledge to optimize the inner loop even further, but it might make the algorithm a little more complicated.

Consider also that each iteration of the inner loop is independent of the others. This means that they can be run out of order or in parallel. Since we're using pointers to reference elements of the state transition lookup tables and state vector arrays, we could easily modify our program to use hardware specific SIMD intrinsics such as those for the SSE or AVX instruction sets. This would allow us to parallelize the computations in the inner loop.

## Counting the Floating Point Operations

The example we worked through in the computation method described above is small enough that we can easily count the number of floating point operations required to compute the result. Here is a table showing the count of all addition and multiplication operations needed to complete each iteration of the outer loop:

{% latex fig-18 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{4.5em}<{$}}
    \newcolumntype{y}{>{$}wr{7.0em}<{$}}
    \newcolumntype{z}{>{$}wr{7.5em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|y|z|}
    \hline
    \text{Iteration}
    &
    \text{Operations ($+$)} & \text{Operations ($\times$)} & \text{Total Operations}
    \\[0.25em]\hline
    k = 1 &  2 &  4 &  6
    \\[0.25em]\hline
    k = 2 &  3 &  6 &  9
    \\[0.25em]\hline
    k = 3 &  4 &  8 & 12
    \\[0.25em]\hline
    k = 4 &  5 & 10 & 15
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can add up the total number of operations for each iteration of the outer loop to arrive at the total number of operations necessary to reach the final result:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    T_4 = 6 + 9 + 12 + 15
    \end{displaymath}
    \end{document}
{% endlatex %}

This tells us how many operations are required for a model with four coin toss events. But what if we're using a much larger model of the coin toss game? The following table shows how to compute the number of operations required for any iteration of the outer loop, regardless of the size of the coin toss model:

{% latex fig-20 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{4.5em}<{$}}
    \newcolumntype{y}{>{$}wr{7.0em}<{$}}
    \newcolumntype{z}{>{$}wr{7.5em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|y|z|}
    \hline
    \text{Iteration}
    &
    \text{Operations ($+$)} & \text{Operations ($\times$)} & \text{Total Operations}
    \\[0.25em]\hline
    k = n &  n + 1 & 2n + 2 & 3n + 3
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now we need to add up the number of operations used in each iteration to get the total number of operations required to compute the final result:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    T_n = \sum_{k = 1}^{n}{(3k + 3)}
    \end{displaymath}
    \end{document}
{% endlatex %}

This formula tells us the total number of floating point operations necessary to calculate the final result for a coin toss model with an arbitrary number of coin toss events. But I think it might be convenient to represent this in algebraic form instead of summation form. Consider the following relationship:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    \sum_{k = 1}^{n}{k} = 1 + 2 + 3 + \dots + n = \frac{n\mspace{1mu}(n + 1)}{2}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the formula for the triangular number sequence. We can use this relationship to replace the summation above and present our solution in the following algebraic form:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    T_n = \frac{3n^2 + 9n}{2}
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, this indicates that our algorithm has quadratic time complexity. But this doesn't tell us everything. How we access memory and whether or not we make efficient use of the cache can have an impact on performance regardless of the number of floating point operations required to complete a task.

## Comparison to Matrix Multiplication

In my earlier post titled [*Generalizing the Coin Toss Markov Model*]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}), we investigated a computation method based on the product of state vectors and state transition matrices. I am curious how this computation method compares to the optimized computation method analyzed in the previous section. For this analysis, we'll use the following notation:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \ell = 2n + 1
    \\[1em]
    &
    \begin{aligned}
    & \mathbf{M}   && \text{is a square $\ell \times \ell$ matrix}
    \\
    & \mathbf{v}_k && \text{is a row vector with $\ell$ elements}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can think of the row vector as a matrix with a single row. Let's start by first counting the number of operations needed to compute the product of two square matrices and the number of operations needed to compute the product of a row vector and a square matrix:

{% latex fig-25 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{7.5em}<{$}}
    \newcolumntype{y}{>{$}wl{5.0em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|y|}
    \hline
    \text{Expression}
    & \mathbf{M} \times \mathbf{M}
    & \mathbf{v}_k \times \mathbf{M}
    \\[0.25em]\hline
    \text{Operations ($+$)}
    & \ell^3 - \ell^2
    & \ell^2 - \ell
    \\[0.25em]\hline
    \text{Operations ($\times$)}
    & \ell^3
    & \ell^2
    \\[0.25em]\hline
    \text{Total Operations}
    & 2 \ell^3 - \ell^2
    & 2 \ell^2 - \ell
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

The number of operations required depends on the size of the matrix. And the size of the matrix depends on the number of coin toss events we are modeling. But the number of matrix operations we need to compute also depends on the number of coin toss events. Recall the following formula from our generalized coin toss Markov model:

{% latex fig-26 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_n = \mathbf{v}_0 \times \mathbf{M}^n
    \end{displaymath}
    \end{document}
{% endlatex %}

If we are modeling a system with four coin toss events, we can expand the above as follows:

{% latex fig-27 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_4
    =
    \mathbf{v}_0 \times \mathbf{M} \times \mathbf{M} \times \mathbf{M} \times \mathbf{M}
    \end{displaymath}
    \end{document}
{% endlatex %}

In this case, there are a total of four matrix operations. Each matrix operation contains many elementary operations. We want to count the number of elementary operations. Since matrix multiplication is associative, we'll get the same result whether we evaluate the expression from left to right or right to left---assuming we don't have any floating point rounding errors. But the number of elementary operations required to evaluate this expression depends on the order in which we perform the evaluation. Here is the analysis for right-associative evaluation:

{% latex fig-28 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{7.5em}<{$}}
    \newcolumntype{y}{>{$}wl{12.0em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|}
    \hline
    \text{Expression}
    & \mathbf{v}_0 \times (\mathbf{M} \times (\mathbf{M} \times (\mathbf{M} \times \mathbf{M})))
    \\[0.25em]\hline
    \text{Operations ($+$)}
    & (n - 1)\ell^3 - (n - 2)\ell^2 - \ell
    \\[0.25em]\hline
    \text{Operations ($\times$)}
    & (n - 1)\ell^3 + \ell^2
    \\[0.25em]\hline
    \text{Total Operations}
    & 2(n - 1)\ell^3 - (n - 3)\ell^2 - \ell
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using this information, we can express the total number of floating point operations as a function of the number of coin toss events:

{% latex fig-29 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    T_n & = 2(n - 1)\ell^3 - (n - 3)\ell^2 - \ell
    \\[1em]
        & = 2(n - 1)(2n + 1)^3 - (n - 3)(2n + 1)^2 - (2n + 1)
    \\[1em]
        & = 16n^4 + 4n^3 - 4n^2 - n
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Thus, our matrix product has a quartic polynomial time complexity when using right-associative evaluation. We can use this formula to compute the total number of elementary operations needed for a model with four coin toss events:

{% latex fig-30 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    T_4 = \text{4,284}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is about two orders of magnitude more than the number of operations required when using the optimized computation method. And the gap is even worse for models with a higher number of coin toss events. But the difference is not as bad if we evaluate the matrix product from left to right instead of right to left. Here is the analysis for left-associative evaluation:

{% latex fig-31 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{7.5em}<{$}}
    \newcolumntype{y}{>{$}wl{12.0em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|}
    \hline
    \text{Expression}
    & (((\mathbf{v}_0 \times \mathbf{M}) \times \mathbf{M}) \times \mathbf{M}) \times \mathbf{M}
    \\[0.25em]\hline
    \text{Operations ($+$)}
    & n\ell^2 - n\ell
    \\[0.25em]\hline
    \text{Operations ($\times$)}
    & n\ell^2
    \\[0.25em]\hline
    \text{Total Operations}
    & 2n\ell^2 - n\ell
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using this information, we can express the total number of floating point operations as a function of the number of coin toss events:

{% latex fig-32 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    T_n & = 2n\ell^2 - n\ell
    \\[1em]
        & = 2n\mspace{1mu}(2n + 1)^2 - n\mspace{1mu}(2n + 1)
    \\[1em]
        & = 8n^3 + 6n^2 + n
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Accordingly, our matrix product has cubic polynomial time complexity when using left-associative evaluation. We can use this formula to compute the total number of elementary operations needed for a model with four coin toss events:

{% latex fig-33 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    T_4 = \text{612}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is a better figure, but it's still more than ten times the number of operations required when using the optimized computation method. And the gap still gets worse for models with a higher number of coin toss events. For each computation method, we can plot the number of operations required as a function of the number of coin toss events to get an idea of what the growth rate of each method looks like:

{% chart fig-34-flop-counts.svg %}

Keep in mind that the vertical axis has a logarithmic scale. As you can see, the optimized computation method scales much better than methods using matrix multiplication. And perhaps this is not surprising when you consider that, in the generalized coin toss model, the state transition matrix is a sparse matrix that contains mostly zeros. Performing addition and multiplication operations against those zero values is a waste of computation resources.

## Comparison to Algebraic Manipulation

Suppose we have a set of algebraic formulas that we can use to compute the expected outcome of the coin toss game given a set of biases. We might be able to calculate the results with fewer operations than any of the methods described above. In an earlier post titled [*Estimating the Weights of Biased Coins*]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}), we derived a set of equations to compute the outcome for a model of the coin toss game with four coin toss events. Let's do something similar here:

{% latex fig-35 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{5em}<{$}|>{$}wl{7em}<{$}|>{$}wl{11em}<{$}|}
    \hline
    \text{Sequence} & \text{Terminal State} & \text{Probability}
    \\[0.25em]\hline
    \texttt{HHHH} & +4 & p_0 \, p_1 \, p_2 \, p_3
    \\[0.25em]\hline
    \texttt{HHHT} & +2 & p_0 \, p_1 \, p_2 \, q_3
    \\[0.25em]\hline
    \texttt{HHTH} & +2 & p_0 \, p_1 \, q_2 \, p_1
    \\[0.25em]\hline
    \texttt{HTHH} & +2 & p_0 \, q_1 \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{THHH} & +2 & p_0 \, q_1 \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{HHTT} & \phantom{+}0 & p_0 \, p_1 \, q_2 \, q_1
    \\[0.25em]\hline
    \texttt{HTHT} & \phantom{+}0 & p_0 \, q_1 \, p_0 \, q_1
    \\[0.25em]\hline
    \texttt{HTTH} & \phantom{+}0 & p_0 \, q_1 \, p_0 \, q_1
    \\[0.25em]\hline
    \texttt{THHT} & \phantom{+}0 & p_0 \, q_1 \, p_0 \, q_1
    \\[0.25em]\hline
    \texttt{THTH} & \phantom{+}0 & p_0 \, q_1 \, p_0 \, q_1
    \\[0.25em]\hline
    \texttt{TTHH} & \phantom{+}0 & p_0 \, p_1 \, q_2 \, q_1
    \\[0.25em]\hline
    \texttt{HTTT} & -2 & p_0 \, q_1 \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{THTT} & -2 & p_0 \, q_1 \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{TTHT} & -2 & p_0 \, p_1 \, q_2 \, p_1
    \\[0.25em]\hline
    \texttt{TTTH} & -2 & p_0 \, p_1 \, p_2 \, q_3
    \\[0.25em]\hline
    \texttt{TTTT} & -4 & p_0 \, p_1 \, p_2 \, p_3
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

With four coin toss events, there are sixteen possible coin toss sequences. The table above shows the probability of each one, along with the terminal state after the final coin toss. We can express the chance of ending up on each one of the final states with the following:

{% latex fig-36 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 4\,\big( p_0 \, q_1 \, p_0 \, q_1 \big)
          + 2\,\big( p_0 \, p_1 \, q_2 \, q_1 \big)
    \\[1em]
    r_2 & = 2\,\big( p_0 \, q_1 \, p_0 \, p_1 \big)
          +    \big( p_0 \, p_1 \, q_2 \, p_1 \big)
          +    \big( p_0 \, p_1 \, p_2 \, q_3 \big)
    \\[1em]
    r_4 & = \phantom{\big(} p_0 \, p_1 \, p_2 \, p_3 \phantom{\big)}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Remember, the coin in the initial state is always a fair coin. The formulas above can be simplified to contain fewer operations:

{% latex fig-37 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = q_1\mspace{1mu}\big( q_1 + p_1\,q_2 \big)
    \\[1em]
    r_2 & = 0.5\,p_1\mspace{1mu}\big( q_1 \,+\, p_1\,q_2 \,+\, p_2\,q_3 \big)
    \\[1em]
    r_4 & = 0.5\,p_1\,p_2\,p_3
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

You might want to stop here and check my work to make sure I did this correctly. It's easy to make a mistake. With these formulas, we can now count all the addition and multiplication operations to get the total number of floating point operations needed to compute the results:

{% latex fig-38 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{4.5em}<{$}}
    \newcolumntype{y}{>{$}wr{7.0em}<{$}}
    \newcolumntype{z}{>{$}wr{7.5em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|y|z|}
    \hline
    \text{Result}
    &
    \text{Operations ($+$)} & \text{Operations ($\times$)} & \text{Total Operations}
    \\[0.25em]\hline
    r_0 &  1 &  2 &  3
    \\[0.25em]\hline
    r_2 &  2 &  4 &  6
    \\[0.25em]\hline
    r_4 &  0 &  3 &  3
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Adding up the total number of operations for each result, we find that, at least in the case where there are four coin toss events, there are fewer operations required than with any of the computation methods examined in the previous sections:

{% latex fig-39 %}
    \begin{document}
    \begin{displaymath}
    T_4 = 3 + 6 + 3
    \end{displaymath}
    \end{document}
{% endlatex %}

It's not clear to me how to generalize this for a model with an arbitrary number of coin toss events. However, it is clear to me that the probability of getting a sequence of all heads or all tails, regardless of the number of coin tosses, can be expressed like this:

{% latex fig-40 %}
    \begin{document}
    \begin{displaymath}
    r_n = \prod_{i = 0}^{n - 1}{p_i}
    \end{displaymath}
    \end{document}
{% endlatex %}

This computation has linear time complexity. The number of operations required is directly proportional to the number of coin toss events. Knowing this, we can assume that an approach using predetermined algebraic formulas has at least a linear growth rate. That's a best case scenario. But realistically, it's probably not that good. Nonetheless, this approach still might have a better performance profile than the optimized computation method we detailed earlier. It might be worth exploring this idea further.

## Performance Bottleneck

The challenge with using the algebraic approach is coming up with the formulas for models with a high number of coin toss events. These formulas also need to be evaluated in a manner that has an acceptable performance profile. In some of the previous posts, I used a [computer algebra library](https://symbolics.mathdotnet.com/) to build up expression trees representing the algebraic formulas. These expression trees were then mapped to a [different expression tree format](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/) and compiled into executable functions.

This method worked beautifully for smaller models of the coin toss game. But the compilation step turned out to be one of the performance bottlenecks preventing this method from being used for larger models of the coin toss game. Furthermore, the executable functions generated by the compilation step didn't run nearly as fast as the optimized computation method. I was also running into stack overflow errors when attempting to solve for larger models. It was unusable for models with more than about twenty coin toss events. I haven't looked too deeply into it yet, but I think I might know what the problem is. Consider the following expression:

{% latex fig-41 %}
    \begin{document}
    \begin{displaymath}
    a + b + c + d
    \end{displaymath}
    \end{document}
{% endlatex %}

This is just a sum of four numbers. For this formula, the expression tree generated by the computer algebra library would look like this:

{% latex fig-42 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows}
    \begin{document}
    \begin{tikzpicture}[x=+0.3333in,y=-0.75in]
    \tikzset{every node/.style={draw,circle,minimum size=0.25in}}
    \tikzset{every path/.style={draw,-stealth',shorten >=1bp}}
    \node (add) at (3, 0) {$+$};
    \node (a)   at (0, 1) {$a$};
    \node (b)   at (2, 1) {$b$};
    \node (c)   at (4, 1) {$c$};
    \node (d)   at (6, 1) {$d$};
    \path (add) -- (a);
    \path (add) -- (b);
    \path (add) -- (c);
    \path (add) -- (d);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

This remains a very flat tree structure regardless of how many numbers we are adding together. Ideally, the sum would be compiled as a loop with an accumulator. But that's not what happens. In preparation for the compilation step, this expression tree gets mapped to a binary expression tree format that looks like this:

{% latex fig-43 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows}
    \begin{document}
    \begin{tikzpicture}[x=+0.5in,y=-0.5in]
    \tikzset{every node/.style={draw,circle,minimum size=0.25in}}
    \tikzset{every path/.style={draw,-stealth',shorten >=1bp}}
    \node (add1) at (1, 2) {$+$};
    \node (add2) at (2, 1) {$+$};
    \node (add3) at (3, 0) {$+$};
    \node (a)    at (0, 3) {$a$};
    \node (b)    at (2, 3) {$b$};
    \node (c)    at (3, 2) {$c$};
    \node (d)    at (4, 1) {$d$};
    \path (add1) -- (a);
    \path (add1) -- (b);
    \path (add2) -- (add1);
    \path (add2) -- (c);
    \path (add3) -- (add2);
    \path (add3) -- (d);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

For complex expressions with many operands, this can be a very deeply nested tree structure. And that's where I think the problem lies. This deep tree structure might be what was causing the compilation step to take a long time. It might also explain why the generated functions ran too slowly and why the evaluation of larger models was exhausting the call stack.

## Hill Climbing with Descending Step Sizes

In some of the examples we looked at in the previous posts, we used a hill climbing algorithm as an optimization technique to find parameters that minimize a cost function. In all of these examples, we used a fixed step size. In the last post, we used a step size that would deliver an accuracy of five decimal places:

{% latex fig-44 %}
    \begin{document}
    \begin{displaymath}
    s_f = 0.00001
    \end{displaymath}
    \end{document}
{% endlatex %}

Consider the examples illustrated for the [linear polynomial method]({% post_url 2021-05-31-approximations-with-polynomials %}#linear-polynomial) in the previous post. Applying the hill climbing algorithm while using this value as the step size, the optimization task took tens of thousands of iterations to complete. We can significantly reduce the number of iterations necessary by using a series of tiered step sizes arranged in descending order:

{% latex fig-45 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    s_1 & = 0.1
    \\
    s_2 & = 0.01
    \\
    s_3 & = 0.001
    \\
    s_4 & = 0.0001
    \\
    s_5 & = 0.00001
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The objective here is to run the hill climbing algorithm to completion using the first step size. Once complete, the process is repeated with the next step size using the result of the previous run as the starting point. We keep repeating this process until we've run the algorithm to completion for the smallest step size. Reproducing the linear polynomial examples from the previous post, here are the paths taken by the hill climbing algorithm when using descending step sizes:

{% chart fig-46-estimate-n-10-1-heatmap.svg %}
{% chart fig-47-estimate-n-10-2-heatmap.svg %}
{% chart fig-48-estimate-n-10-3-heatmap.svg %}
{% chart fig-49-estimate-n-10-4-heatmap.svg %}

Except for the last one, which drifts off into a local minimum, all paths finish with the same result. And this result is the same one we found when using a fixed step size. But when using descending step sizes, the results converge in far fewer iterations. Here is a comparison:

{% latex fig-50 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{>{$}wl{3.0em}<{$}}
    \newcolumntype{y}{>{$}wr{8.5em}<{$}}
    \begin{array}{@{\rule{0em}{1.25em}}|x|y|y|}
    \hline
    \text{Trace}
    & \text{Iterations ($s_f$)}
    & \text{Iterations ($s_1$---$s_5$)}
    \\[0.25em]\hline
    1 & \text{60,351} & \text{17}
    \\[0.25em]\hline
    2 & \text{48,205} & \text{19}
    \\[0.25em]\hline
    3 & \text{41,829} & \text{17}
    \\[0.25em]\hline
    4 & \text{32,355} & \text{9}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

The difference is off the charts. Beginning the hill climbing method with a large step size allows the process to move quickly towards the target area, while the smaller step sizes enable it to zero in on a precise result. The benefits are clear. And this technique might be useful for other optimization methods as well.

## Example with 20 Coin Tosses

With the performance enhancements outlined in the sections above, we can now apply the polynomial approximation technique to larger models of the coin toss game. Using a model of the coin toss game with twenty coin toss events, let's use the [quadratic polynomial approximation]({% post_url 2021-05-31-approximations-with-polynomials %}#quadratic-polynomial) technique described in the previous post to find a set of weights that approximate the following target distribution:

{% chart fig-51-target-pmfunc-n-20.svg %}

Starting with a set of fair coins for the initial guess, we can apply the hill climbing algorithm to find an optimal set of weights for the biased coins. The process completes after 25 iterations. Here are the results:

{% chart fig-52-estimate-n-20-5-biases-fitted.svg %}
{% chart fig-53-estimate-n-20-5-pmfunc-fitted.svg %}

The results are computed almost instantaneously. Without the optimized computation method, these calculations would have taken about twenty seconds to run on my current hardware. And without the descending step size optimization, this task would have taken about thirty minutes to complete.

## Example with 50 Coin Tosses

Let's do another example. In this one, we'll use the [cubic polynomial approximation]({% post_url 2021-05-31-approximations-with-polynomials %}#cubic-polynomial) technique on a model with fifty coin toss events. A model this size would be impractical or impossible to evaluate without the performance optimizations chronicled in this post. Here is the target distribution we want to approximate:

{% chart fig-54-target-pmfunc-n-50.svg %}

Starting with a set of fair coins for the initial guess, we can apply the hill climbing algorithm to find an optimal set of weights for the biased coins. The process completes after 1,746 iterations. Here are the results:

{% chart fig-55-estimate-n-50-6-biases-fitted.svg %}
{% chart fig-56-estimate-n-50-6-pmfunc-fitted.svg %}

This is a pretty good approximation, but it is not the most optimal result that can be found with a cubic polynomial. When using fair coins for the initial guess, the hill climbing method takes a route that leads to a local minimum. Also, notice that the number of iterations required is two orders of magnitude more than the other examples in the last two sections. I have some ideas for improving this technique further, but I will save that discussion for another time.

{% accompanying_src_link %}
