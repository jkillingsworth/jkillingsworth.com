---
layout: post
title: More Coin Toss Performance Enhancements
---

This post is an extension of the [previous post]({% post_url 2021-07-12-performance-tuning-for-the-coin-toss-model %}) in which I explored some techniques for speeding up the calculations used to find approximate solutions to the coin toss problem. Here I want to examine a couple of enhancements to these ideas. First, I describe an enhanced computation method that cuts the number of floating point operations required almost in half. Second, I introduce a progressive polynomial approximation technique that can reduce the number of iterations needed to find a solution.

<!--excerpt-->

## Enhanced Computation Method

The optimized computation method outlined in the previous post had a much better performance profile than the previously used method. However, it still included a bunch of redundant computations. For even numbered coin tosses, the value computed for the odd numbered states is always zero. Likewise, for odd numbered coin tosses, the value computed for the even numbered states is always zero. Since we know these alternating values are always zero, we really don't need to compute them. We can use an enhanced computation method that just eliminates the unnecessary computations. Like before, the best way to describe how it works is with an example. Suppose we have a model of the coin toss game with five coin toss events per round:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    n = 5
    \end{displaymath}
    \end{document}
{% endlatex %}

Since we're using the same symmetrical coin toss model we've been using in the past few posts, we can assume that the coin in the initial state is always a fair coin:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_0 & = 0.5000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

For this example, let's choose some arbitrary values for the remaining state transitions:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.4000
    \\
    p_2 & = 0.3000
    \\
    p_3 & = 0.2000
    \\
    p_4 & = 0.1000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to create some lookup tables for our state transition values like we did for the optimized computation method presented in the previous post. But this time around, we want to create two different sets of lookup tables, one for odd numbered coin tosses and one for even numbered coin tosses. Here is the formula to populate the lookup tables for odd numbered coin tosses:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \begin{drcases}
    \mathrlap{u_{i}}\phantom{u_{i}} = p_{(2i)}\quad
    \\[0.5em]
    \mathrlap{v_{i}}\phantom{u_{i}} = 1 - p_{(2i)}\quad
    \end{drcases}
    \quad \forall i \in \left\{\, 0, \dots, \left\lfloor \tfrac{n - 1}{2} \right\rfloor \,\right\}
    \end{displaymath}
    \end{document}
{% endlatex %}

This formula is based on the even numbered state transition values. Here is what the populated arrays wind up looking like:

{% latex fig-05 %}
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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{u}$};
    \node[cell={frame,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={frame,inner,value}] (vp0)   at ( 0,   1.0) {0.5000};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {0.3000};
    \node[cell={frame,inner,value}]         at ( 2,   1.0) {0.1000};
    \node[cell={frame,outer,empty}]         at ( 3,   1.0) {0};
    \node[pnode]                    (pp0)   at ( 0,   2.7) {$u_{0}$};
    \path[ppath]                    (pp0)   -- (vp0);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{v}$};
    \node[cell={frame,outer,empty}]         at (-1,   4.5) {0};
    \node[cell={frame,inner,value}] (vq0)   at ( 0,   4.5) {0.5000};
    \node[cell={frame,inner,value}]         at ( 1,   4.5) {0.7000};
    \node[cell={frame,inner,value}]         at ( 2,   4.5) {0.9000};
    \node[cell={frame,outer,empty}]         at ( 3,   4.5) {0};
    \node[pnode]                    (pq0)   at ( 0,   6.2) {$v_{0}$};
    \path[ppath]                    (pq0)   -- (vq0);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

We need to align the arrays in a way that makes it easy to perform the calculations. In this case, we only need to shift one of them to the left:

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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{u}$};
    \node[cell={frame,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={frame,inner,value}] (vph0)  at ( 0,   1.0) {0.5000};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {0.3000};
    \node[cell={frame,inner,value}]         at ( 2,   1.0) {0.1000};
    \node[cell={frame,outer,empty}]         at ( 3,   1.0) {0};
    \node[pnode]                    (pph0)  at ( 0,   2.7) {$u^{\sscr{heads}}_{0}$};
    \path[ppath]                    (pph0)  -- (vph0);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{v}$};
    \node[edge={frame,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={frame,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={frame,inner,value}] (vqt0)  at ( 0,   4.5) {0.7000};
    \node[cell={frame,inner,value}]         at ( 1,   4.5) {0.9000};
    \node[cell={frame,inner,empty}]         at ( 2,   4.5) {0};
    \node[pnode]                    (pqt0)  at ( 0,   6.2) {$v^{\sscr{tails}}_{0}$};
    \path[ppath]                    (pqt0)  -- (vqt0);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

These are the lookup tables we'll use for odd numbered coin tosses. For even numbered coin tosses, we need to do something similar. But we need to do it with the alternate set of state transition values. Here is the formula to populate the lookup tables for even numbered coin tosses:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \begin{drcases}
    \mathrlap{x_{i}}\phantom{x_{i}} = p_{(2i + 1)}\quad
    \\[0.5em]
    \mathrlap{y_{i}}\phantom{y_{i}} = 1 - p_{(2i + 1)}\quad
    \end{drcases}
    \quad \forall i \in \left\{\, 0, \dots, \left\lfloor \tfrac{n - 2}{2} \right\rfloor \,\right\}
    \end{displaymath}
    \end{document}
{% endlatex %}

This formula is based on the odd numbered state transition values. Here is what the populated arrays wind up looking like:

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
    \node[index]                            at (-1,  -0.6) {-1};
    \node[index]                            at ( 0,  -0.6) {0};
    \node[index]                            at ( 1,  -0.6) {1};
    \node[index]                            at ( 2,  -0.6) {2};
    \node[index]                            at ( 3,  -0.6) {3};
    \node[array]                            at (-1.9, 1.0) {$\mathbf{x}$};
    \node[cell={frame,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={frame,inner,value}] (vp0)   at ( 0,   1.0) {0.4000};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {0.2000};
    \node[cell={frame,inner,empty}]         at ( 2,   1.0) {0};
    \node[cell={frame,outer,empty}]         at ( 3,   1.0) {0};
    \node[pnode]                    (pp0)   at ( 0,   2.7) {$x_{0}$};
    \path[ppath]                    (pp0)   -- (vp0);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{y}$};
    \node[cell={frame,outer,empty}]         at (-1,   4.5) {0};
    \node[cell={frame,inner,value}] (vq0)   at ( 0,   4.5) {0.6000};
    \node[cell={frame,inner,value}]         at ( 1,   4.5) {0.8000};
    \node[cell={frame,inner,empty}]         at ( 2,   4.5) {0};
    \node[cell={frame,outer,empty}]         at ( 3,   4.5) {0};
    \node[pnode]                    (pq0)   at ( 0,   6.2) {$y_{0}$};
    \path[ppath]                    (pq0)   -- (vq0);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

We need to align the arrays in a way that makes it easy to perform the calculations. In this case, we only need to shift one of them to the right:

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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{x}$};
    \node[cell={frame,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={frame,inner,value}]         at ( 1,   1.0) {0.4000};
    \node[cell={frame,inner,value}]         at ( 2,   1.0) {0.2000};
    \node[cell={frame,outer,empty}]         at ( 3,   1.0) {0};
    \node[edge={frame,outer,R}]             at ( 3.6, 1.0) {};
    \node[pnode]                    (pph0)  at ( 0,   2.7) {$x^{\sscr{heads}}_{0}$};
    \path[ppath]                    (pph0)  -- (vph0);
    \node[array]                            at (-1.9, 4.5) {$\mathbf{y}$};
    \node[cell={frame,outer,empty}]         at (-1,   4.5) {0};
    \node[cell={frame,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={frame,inner,value}]         at ( 1,   4.5) {0.8000};
    \node[cell={frame,inner,empty}]         at ( 2,   4.5) {0};
    \node[cell={frame,outer,empty}]         at ( 3,   4.5) {0};
    \node[pnode]                    (pqt0)  at ( 0,   6.2) {$y^{\sscr{tails}}_{0}$};
    \path[ppath]                    (pqt0)  -- (vqt0);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

These are the lookup tables we'll use for even numbered coin tosses. Notice the padded zeros in the front and back of the arrays. This padding guarantees that we can shift the arrays left or right without violating any memory allocated for something else. For this method, we also need to shift the state vector arrays left and right in a similar fashion when computing the results for the even and odd coin tosses. Here is the algorithm for the enhanced computation method:

{% latex fig-10 %}
    \usepackage[vlined]{algorithm2e}
    \begin{document}
    \begin{algorithm}[H]
    \DontPrintSemicolon
    \For{$k = 1$ \KwTo $n$}{
        \BlankLine
        $j = k - 1$\;
        \BlankLine
        \For{$i = 0$ \KwTo $\left\lfloor \frac{k}{2} \right\rfloor$}{
            \BlankLine
            \uIf{$(k \bmod 2) = 1$}{
                \BlankLine
                $
                r_{k,i}
                \leftarrow
                \big( u_{i} \cdot r_{j,i} \big)
                +
                \big( v_{(i + 1)} \cdot r_{j,(i + 1)} \big)
                $\;
                \BlankLine
            }
            \Else{
                \BlankLine
                $
                r_{k,i}
                \leftarrow
                \big( x_{(i - 1)} \cdot r_{j,(i - 1)} \big)
                +
                \big( y_{i} \cdot r_{j,i} \big)
                $\;
                \BlankLine
            }
        }
        \BlankLine
        \If{$(k \bmod 2) = 0$}{
            $r_{k,0} \leftarrow 2 \cdot r_{k,0}$\;
        }
    }
    \end{algorithm}
    \end{document}
{% endlatex %}

The execution path of the inner loop depends on whether we are calculating the result for an even numbered coin toss or an odd numbered coin toss. Notice that we double the value in the zero offset just like we did for the optimized computation method in the previous post. But in this case, we only do it for even numbered coin tosses.

Here are the computed values after outer loop iteration #1:

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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{u}$};
    \node[cell={minor,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={minor,inner,value}] (vph0)  at ( 0,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.1000};
    \node[cell={minor,outer,empty}]         at ( 3,   1.0) {0};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{0}$};
    \node[cell={minor,outer,empty}]         at (-1,   2.5) {0};
    \node[cell={minor,inner,value}] (vrh00) at ( 0,   2.5) {1.0000};
    \node[cell={minor,inner,empty}]         at ( 1,   2.5) {0};
    \node[cell={minor,inner,empty}]         at ( 2,   2.5) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   2.5) {0};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{v}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.9000};
    \node[cell={minor,inner,empty}]         at ( 2,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{0}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {1.0000};
    \node[cell={minor,inner,empty}] (vrt00) at ( 0,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 1,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{1}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr10)  at ( 0,   8.0) {0.5000};
    \node[cell={major,inner,empty}]         at ( 1,   8.0) {0};
    \node[cell={major,inner,empty}]         at ( 2,   8.0) {0};
    \node[cell={major,outer,empty}]         at ( 3,   8.0) {0};
    \node[pnode]                    (pr10)  at ( 0,   9.7) {$r_{1,0}$};
    \path[ppath]                    (pr10)  -- (vr10);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #2:

{% latex fig-12 %}
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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{x}$};
    \node[cell={minor,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.4000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.2000};
    \node[cell={minor,outer,empty}]         at ( 3,   1.0) {0};
    \node[edge={minor,outer,R}]             at ( 3.6, 1.0) {};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{1}$};
    \node[cell={minor,inner,empty}] (vrh10) at ( 0,   2.5) {0};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.5000};
    \node[cell={minor,inner,empty}]         at ( 2,   2.5) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   2.5) {0};
    \node[edge={minor,outer,R}]             at ( 3.6, 2.5) {};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{y}$};
    \node[cell={minor,outer,empty}]         at (-1,   4.5) {0};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.8000};
    \node[cell={minor,inner,empty}]         at ( 2,   4.5) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{1}$};
    \node[cell={minor,outer,empty}]         at (-1,   6.0) {0};
    \node[cell={minor,inner,value}] (vrt10) at ( 0,   6.0) {0.5000};
    \node[cell={minor,inner,empty}]         at ( 1,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{2}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr20)  at ( 0,   8.0) {0.6000};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.2000};
    \node[cell={major,inner,empty}]         at ( 2,   8.0) {0};
    \node[cell={major,outer,empty}]         at ( 3,   8.0) {0};
    \node[pnode]                    (pr20)  at ( 0,   9.7) {$r_{2,0}$};
    \path[ppath]                    (pr20)  -- (vr20);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #3:

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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{u}$};
    \node[cell={minor,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={minor,inner,value}] (vph0)  at ( 0,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.1000};
    \node[cell={minor,outer,empty}]         at ( 3,   1.0) {0};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{2}$};
    \node[cell={minor,outer,empty}]         at (-1,   2.5) {0};
    \node[cell={minor,inner,value}] (vrh20) at ( 0,   2.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.2000};
    \node[cell={minor,inner,empty}]         at ( 2,   2.5) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   2.5) {0};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{v}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.9000};
    \node[cell={minor,inner,empty}]         at ( 2,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{2}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {0.6000};
    \node[cell={minor,inner,value}] (vrt20) at ( 0,   6.0) {0.2000};
    \node[cell={minor,inner,empty}]         at ( 1,   6.0) {0};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{3}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr30)  at ( 0,   8.0) {0.4400};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.0600};
    \node[cell={major,inner,empty}]         at ( 2,   8.0) {0};
    \node[cell={major,outer,empty}]         at ( 3,   8.0) {0};
    \node[pnode]                    (pr30)  at ( 0,   9.7) {$r_{3,0}$};
    \path[ppath]                    (pr30)  -- (vr30);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #4:

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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{x}$};
    \node[cell={minor,inner,empty}] (vph0)  at ( 0,   1.0) {0};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.4000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.2000};
    \node[cell={minor,outer,empty}]         at ( 3,   1.0) {0};
    \node[edge={minor,outer,R}]             at ( 3.6, 1.0) {};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{3}$};
    \node[cell={minor,inner,empty}] (vrh30) at ( 0,   2.5) {0};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.4400};
    \node[cell={minor,inner,value}]         at ( 2,   2.5) {0.0600};
    \node[cell={minor,outer,empty}]         at ( 3,   2.5) {0};
    \node[edge={minor,outer,R}]             at ( 3.6, 2.5) {};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{y}$};
    \node[cell={minor,outer,empty}]         at (-1,   4.5) {0};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.6000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.8000};
    \node[cell={minor,inner,empty}]         at ( 2,   4.5) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{3}$};
    \node[cell={minor,outer,empty}]         at (-1,   6.0) {0};
    \node[cell={minor,inner,value}] (vrt30) at ( 0,   6.0) {0.4400};
    \node[cell={minor,inner,value}]         at ( 1,   6.0) {0.0600};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[cell={minor,outer,empty}]         at ( 3,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{4}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr40)  at ( 0,   8.0) {0.5280};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.2240};
    \node[cell={major,inner,write}]         at ( 2,   8.0) {0.0120};
    \node[cell={major,outer,empty}]         at ( 3,   8.0) {0};
    \node[pnode]                    (pr40)  at ( 0,   9.7) {$r_{4,0}$};
    \path[ppath]                    (pr40)  -- (vr40);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

Here are the computed values after outer loop iteration #5:

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
    \node[array]                            at (-1.9, 1.0) {$\mathbf{u}$};
    \node[cell={minor,outer,empty}]         at (-1,   1.0) {0};
    \node[cell={minor,inner,value}] (vph0)  at ( 0,   1.0) {0.5000};
    \node[cell={minor,inner,value}]         at ( 1,   1.0) {0.3000};
    \node[cell={minor,inner,value}]         at ( 2,   1.0) {0.1000};
    \node[cell={minor,outer,empty}]         at ( 3,   1.0) {0};
    \node[array]                            at (-1.9, 2.5) {$\mathbf{r}_{4}$};
    \node[cell={minor,outer,empty}]         at (-1,   2.5) {0};
    \node[cell={minor,inner,value}] (vrh40) at ( 0,   2.5) {0.5280};
    \node[cell={minor,inner,value}]         at ( 1,   2.5) {0.2240};
    \node[cell={minor,inner,value}]         at ( 2,   2.5) {0.0120};
    \node[cell={minor,outer,empty}]         at ( 3,   2.5) {0};
    \node[array]                            at (-1.9, 4.5) {$\mathbf{v}$};
    \node[edge={minor,outer,L}]             at (-1.6, 4.5) {};
    \node[cell={minor,outer,value}]         at (-1,   4.5) {0.5000};
    \node[cell={minor,inner,value}] (vqt0)  at ( 0,   4.5) {0.7000};
    \node[cell={minor,inner,value}]         at ( 1,   4.5) {0.9000};
    \node[cell={minor,inner,empty}]         at ( 2,   4.5) {0};
    \node[array]                            at (-1.9, 6.0) {$\mathbf{r}_{4}$};
    \node[edge={minor,outer,L}]             at (-1.6, 6.0) {};
    \node[cell={minor,outer,value}]         at (-1,   6.0) {0.5280};
    \node[cell={minor,inner,value}] (vrt40) at ( 0,   6.0) {0.2240};
    \node[cell={minor,inner,value}]         at ( 1,   6.0) {0.0120};
    \node[cell={minor,inner,empty}]         at ( 2,   6.0) {0};
    \node[array]                            at (-1.9, 8.0) {$\mathbf{r}_{5}$};
    \node[cell={major,outer,empty}]         at (-1,   8.0) {0};
    \node[cell={major,inner,write}] (vr50)  at ( 0,   8.0) {0.4208};
    \node[cell={major,inner,write}]         at ( 1,   8.0) {0.0780};
    \node[cell={major,inner,write}]         at ( 2,   8.0) {0.0012};
    \node[cell={major,outer,empty}]         at ( 3,   8.0) {0};
    \node[pnode]                    (pr50)  at ( 0,   9.7) {$r_{5,0}$};
    \path[ppath]                    (pr50)  -- (vr50);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

After the loop terminates, we have the probabilities of landing on each state after the fifth and final toss of the coin. This result only includes the terminal states since we never bothered to compute the non terminal states. Since this example models an odd number of coin toss events, the terminal states are always odd numbered states. Here are the resulting values:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = 0.4208
    \\
    r_3 & = 0.0780
    \\
    r_5 & = 0.0012
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using this enhanced computation method, we can compute the final result with less memory and fewer floating point operations than we could with the optimized computation method presented in the last post.

## Counting the Floating Point Operations

How does the number of floating point operations required for the enhanced computation method presented above compare to the number of floating point operations required for the optimized computation method presented in the previous post? Let's count the number of operations needed for each iteration of the outer loop in the above example:

{% latex fig-17 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{4.5em}|wr{7em}|wr{7em}|wr{7.5em}|}
    \hline
    \text{Iteration}
    &
    \text{Operations ($+$)} & \text{Operations ($\times$)} & \text{Total Operations}
    \\[0.25em]\hline
    k = 1 &  1 &  2 &  3
    \\[0.25em]\hline
    k = 2 &  2 &  5 &  7
    \\[0.25em]\hline
    k = 3 &  2 &  4 &  6
    \\[0.25em]\hline
    k = 4 &  3 &  7 & 10
    \\[0.25em]\hline
    k = 5 &  3 &  6 &  9
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

From the table above, we can deduce the following generalization for each iteration, regardless of the size of the coin toss model:

{% latex fig-18 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{4.5em}|wr{7em}|wr{7em}|wr{7.5em}|}
    \hline
    \text{Iteration}
    &
    \text{Operations ($+$)} & \text{Operations ($\times$)} & \text{Total Operations}
    \\[0.25em]\hline
    k \text{ (even)} & \frac{1}{2} (k + 2) & k + 3 & \frac{1}{2} (3k + 8)
    \\[0.25em]\hline
    k \text{ (odd)}  & \frac{1}{2} (k + 1) & k + 1 & \frac{1}{2} (3k + 3)
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can sum the number of operations for each iteration to get the total number of operations for a coin toss model of any size. Here is the formula:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    T_n = \sum_{k = 1}^{n}{\frac{(3k + 3)}{2}} + \frac{5 \left\lfloor \frac{n}{2} \right\rfloor}{2}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can replace the summation like we did in the last post and present the solution in the following algebraic form:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    T_n = \frac{3n^2 + 9n + 10 \left\lfloor \frac{n}{2} \right\rfloor}{4}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using the optimized computation method from the last post as a baseline, we can see how well the enhanced computation method stacks up in comparison:

{% chart fig-21-flop-counts.svg %}

Both methods have quadratic time complexity. But as you can see in the chart above, the enhanced computation method requires fewer floating point operations in all cases. The enhanced computation method requires roughly half the number of floating point operations.

## Progressive Polynomial Approximation

In the last post, we looked at [an example]({% post_url 2021-07-12-performance-tuning-for-the-coin-toss-model %}#example-with-50-coin-tosses) of using the cubic polynomial approximation technique to find an approximate solution for a model with fifty coin toss events. The method took 1,746 iterations to complete. Using a progressive polynomial approximation technique as an alternative, we can arrive at a solution in less than a tenth of the number of iterations. To see how it works, let's work through the example using the alternative approach. We'll use the same target distribution that we did before:

{% chart fig-22-target-pmfunc-n-50.svg %}

Like we did before, we'll start with a set of fair coins for the initial guess. And like before, we'll use the hill climbing method to find an optimal set of weights that can be described by a polynomial. But instead of fitting a cubic polynomial, we'll start by fitting a constant polynomial. Here is the result after 10 iterations:

{% chart fig-23-estimate-n-50-0-biases-fitted.svg %}

This result can now be used as the starting point for the next step in the process. We'll follow the same procedure using a linear polynomial instead of a constant polynomial. Here is the result after an additional 49 iterations:

{% chart fig-24-estimate-n-50-1-biases-fitted.svg %}

Repeating the procedure once again, we can use the result above as the starting point for finding the most optimal solution that can be described by a quadratic polynomial. Here is the result after another 26 iterations:

{% chart fig-25-estimate-n-50-2-biases-fitted.svg %}

And repeating this procedure one more time, we can use the result above as the starting point for the final stage in the process---finding the best result that can be described by a cubic polynomial. Here is the result after 28 more iterations:

{% chart fig-26-estimate-n-50-3-biases-fitted.svg %}

This is the final solution after a total of 113 iterations. In this case, using the progressive polynomial approximation technique allows us to find a solution that fits a cubic polynomial in far fewer iterations than the regular cubic polynomial approximation method. In addition, it is worth noting that iterations involving lower order polynomials are less computationally expensive.

You might notice that this is not the same solution that was found using the regular cubic polynomial approximation technique in the previous post. And it is not the most optimal solution that can be found with a cubic polynomial. Like the example in the last post, the hill climbing process gets stuck in what seems to be a local minimum. But further analysis indicates that it might be stuck in a narrow ridge or valley. Applying the hill climbing algorithm with smaller step sizes may lead to a more accurate solution, but there is no guarantee. Because of this, I am a bit wary of using the hill climbing algorithm. I might experiment with using a different approach to avoid this pitfall.

{% accompanying_src_link %}
