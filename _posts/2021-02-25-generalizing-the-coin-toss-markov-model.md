---
layout: post
title: Generalizing the Coin Toss Markov Model
---

This is a continuation of the series on weighted coin toss games. In previous posts, we explored variations of the weighted coin toss game using two, three, and four flips per round. In each variation, the game was described using a Markov model with a fixed number of coin toss events. This post presents a generalized form of the Markov model that can be used to model a game with an arbitrary number of coin toss events. I also show a few examples using a model of the coin toss game with ten flips per round.

<!--excerpt-->

## Markov Model with 2 Coin Tosses

Let's start with a very simple model of the coin toss game that uses only two flips of the coin per round. This is the model used in the previous post titled [*Visualizing Saddle Points and Minimums*]({% post_url 2021-01-29-visualizing-saddle-points-and-minimums %}). Here is what the Markov model looks like:

{% latex fig-01 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,automata}
    \begin{document}
    \begin{tikzpicture}[auto,>=stealth',shorten >=1bp,node distance=1.125in]
    \tikzset{every state/.style={minimum size=0.5in}}
    \node[state,initial right] (00)               {$S_0$};
    \node[state]               (+1) [above of=00] {$S_{+1}$};
    \node[state]               (+2) [right of=+1] {$S_{+2}$};
    \node[state]               (-1) [below of=00] {$S_{-1}$};
    \node[state]               (-2) [right of=-1] {$S_{-2}$};
    \path[->]
    (00) edge              node [swap] {$  p_0$} (+1)
    (+1) edge [bend right] node [swap] {$1-p_1$} (00)
    (+1) edge              node        {$  p_1$} (+2)
    (00) edge              node        {$  p_0$} (-1)
    (-1) edge [bend left]  node        {$1-p_1$} (00)
    (-1) edge              node        {$  p_1$} (-2);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

In this model, there are five possible states that the system can be in and six possible state transitions. Each arrow represents a state transition. The state diagram above can be represented using a state transition matrix:

{% latex fig-02 %}
    \usepackage{array}
    \begin{document}
    \begin{displaymath}
    \mathbf{M} =
    \left[
    \newcolumntype{x}{>{$}wl{2.75em}<{$}}
    \begin{array}{xxxxl}
    0   & 0   & 0       & 0   & 0
    \\[1em]
    p_1 & 0   & 1 - p_1 & 0   & 0
    \\[1em]
    0   & p_0 & 0       & p_0 & 0
    \\[1em]
    0   & 0   & 1 - p_1 & 0   & p_1
    \\[1em]
    0   & 0   & 0       & 0   & 0
    \end{array}
    \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

This transition matrix determines the probability of moving from one state to the next. This is a square matrix with a row and a column for each state. The rows represent the starting states, and the columns represent the subsequent states. We can also use a vector with five elements---one for each state---to represent the probability of being in each one of the states at a particular point in time. Since we always start in the zero state, the initial vector looks like this:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_0 =
    \left[
    \begin{array}{l}
    0
    \\[1em]
    0
    \\[1em]
    1
    \\[1em]
    0
    \\[1em]
    0
    \end{array}
    \right]^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can compute the probability of being in each one of the five states after the first coin toss by taking the product of the state vector and the state transition matrix:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_1 = \mathbf{v}_0 \times \mathbf{M}
    \end{displaymath}
    \end{document}
{% endlatex %}

After the first coin toss, there are two possible states that the system can be in. The product above works out to the following:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_1 =
    \left[
    \begin{array}{l}
    0
    \\[1em]
    p_0
    \\[1em]
    0
    \\[1em]
    p_0
    \\[1em]
    0
    \end{array}
    \right]^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since there are two flips of the coin per round, we can compute the final outcome distribution by multiplying the vector above by the transition matrix one more time:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_2 = \mathbf{v}_1 \times \mathbf{M}
    \end{displaymath}
    \end{document}
{% endlatex %}

After the second coin toss, there are three possible states the system can be in. The product above works out to the following:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_2 =
    \left[
    \begin{array}{l}
    p_0 \, p_1
    \\[1em]
    0
    \\[1em]
    2 \, p_0 \, (1 - p_1)
    \\[1em]
    0
    \\[1em]
    p_0 \, p_1
    \end{array}
    \right]^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, for the final outcome, the system can only be in one of three out of the five possible states after the second coin toss. The other two states only serve as intermediate states that the system transitions through. Note that there is a possibility that the system returns to the initial state after the second coin toss.

## Markov Model with 3 Coin Tosses

A slightly more complicated model is that of a coin toss game with three flips of the coin per round. This is the model used previously in the post titled [*Visualizing the Climb up the Hill*]({% post_url 2020-08-16-visualizing-the-climb-up-the-hill %}). Here is what the Markov model looks like:

{% latex fig-08 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,automata}
    \begin{document}
    \begin{tikzpicture}[auto,>=stealth',shorten >=1bp,node distance=1.125in]
    \tikzset{every state/.style={minimum size=0.5in}}
    \node[state,initial right] (00)               {$S_0$};
    \node[state]               (+1) [above of=00] {$S_{+1}$};
    \node[state]               (+2) [right of=+1] {$S_{+2}$};
    \node[state]               (+3) [right of=+2] {$S_{+3}$};
    \node[state]               (-1) [below of=00] {$S_{-1}$};
    \node[state]               (-2) [right of=-1] {$S_{-2}$};
    \node[state]               (-3) [right of=-2] {$S_{-3}$};
    \path[->]
    (00) edge              node [swap] {$  p_0$} (+1)
    (+1) edge [bend right] node [swap] {$1-p_1$} (00)
    (+1) edge [bend left]  node        {$  p_1$} (+2)
    (+2) edge [bend left]  node        {$1-p_2$} (+1)
    (+2) edge              node        {$  p_2$} (+3)
    (00) edge              node        {$  p_0$} (-1)
    (-1) edge [bend left]  node        {$1-p_1$} (00)
    (-1) edge [bend left]  node        {$  p_1$} (-2)
    (-2) edge [bend left]  node        {$1-p_2$} (-1)
    (-2) edge              node        {$  p_2$} (-3);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

In this model, there are seven possible states that the system can be in and a total of ten possible state transitions. The state diagram illustrated above can be represented with the following state transition matrix:

{% latex fig-09 %}
    \usepackage{array}
    \begin{document}
    \begin{displaymath}
    \mathbf{M} =
    \left[
    \newcolumntype{x}{>{$}wl{2.75em}<{$}}
    \begin{array}{xxxxxxl}
    0   & 0   & 0       & 0       & 0       & 0   & 0
    \\[1em]
    p_2 & 0   & 1 - p_2 & 0       & 0       & 0   & 0
    \\[1em]
    0   & p_1 & 0       & 1 - p_1 & 0       & 0   & 0
    \\[1em]
    0   & 0   & p_0     & 0       & p_0     & 0   & 0
    \\[1em]
    0   & 0   & 0       & 1 - p_1 & 0       & p_1 & 0
    \\[1em]
    0   & 0   & 0       & 0       & 1 - p_2 & 0   & p_2
    \\[1em]
    0   & 0   & 0       & 0       & 0       & 0   & 0
    \end{array}
    \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

This is a square matrix with seven rows and seven columns. We can also use a seven element vector to represent the likelihood of the system being in a particular state at a given point in time. The initial vector looks like this:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_0 =
    \left[
    \begin{array}{l}
    0
    \\[1em]
    0
    \\[1em]
    0
    \\[1em]
    1
    \\[1em]
    0
    \\[1em]
    0
    \\[1em]
    0
    \end{array}
    \right]^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can multiply this vector by the transition matrix three times to determine where we might find the state of the system after three flips of the coin:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_3 = \mathbf{v}_0 \times \mathbf{M} \times \mathbf{M} \times \mathbf{M}
    \end{displaymath}
    \end{document}
{% endlatex %}

After the third coin toss, there are four possible states the system can be in. The product above works out to the following:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_3 =
    \left[
    \begin{array}{l}
    p_0 \, p_1 \, p_2
    \\[1em]
    0
    \\[1em]
    p_0 \, p_1 \, (1 - p_2) \, + \, 2 \, p_0 \, (1 - p_1) \, p_0
    \\[1em]
    0
    \\[1em]
    2 \, p_0 \, (1 - p_1) \, p_0 \, + \, p_0 \, p_1 \, (1 - p_2)
    \\[1em]
    0
    \\[1em]
    p_0 \, p_1 \, p_2
    \end{array}
    \right]^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

The system can be in one of four possible states for the final outcome. The other three states are intermediate states that the system transitions through. Notice that the initial state is not one of the final states since there is an odd number of coin tosses in this case.

## Generalized Markov Model

In addition to the two Markov models outlined above, you can also find a detailed discussion of a model of the coin toss game with four flips per round in one of my earlier posts titled [*Estimating the Weights of Biased Coins*]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}). All of these models can be described by a generalized model. Suppose we have a coin toss game with an arbitrary number of flips per round. We can represent the generalized form of the Markov model with a state diagram that looks like this:

{% latex fig-13 %}
    \usepackage{tikz}
    \usetikzlibrary{arrows,automata}
    \begin{document}
    \begin{tikzpicture}[auto,>=stealth',shorten >=1bp,node distance=1.125in]
    \tikzset{every state/.style={minimum size=0.5in}}
    \node[state,initial right] (00)               {$S_0$};
    \node[state]               (+1) [above of=00] {$S_{+1}$};
    \node[state,draw=none]     (+2) [right of=+1] {$\dots$};
    \node[state]               (+3) [right of=+2] {$S_{+n-1}$};
    \node[state]               (+4) [right of=+3] {$S_{+n}$};
    \node[state]               (-1) [below of=00] {$S_{-1}$};
    \node[state,draw=none]     (-2) [right of=-1] {$\dots$};
    \node[state]               (-3) [right of=-2] {$S_{-n+1}$};
    \node[state]               (-4) [right of=-3] {$S_{-n}$};
    \path[->]
    (00) edge              node [swap] {$  p_0$} (+1)
    (+1) edge [bend right] node [swap] {$1-p_1$} (00)
    (+1) edge [bend left]  node        {$  p_1$} (+2)
    (+2) edge [bend left]  node        {$1-p_2$} (+1)
    (+2) edge [bend left]  node        {$  p_{n-2}$} (+3)
    (+3) edge [bend left]  node        {$1-p_{n-1}$} (+2)
    (+3) edge              node        {$  p_{n-1}$} (+4)
    (00) edge              node        {$  p_0$} (-1)
    (-1) edge [bend left]  node        {$1-p_1$} (00)
    (-1) edge [bend left]  node        {$  p_1$} (-2)
    (-2) edge [bend left]  node        {$1-p_2$} (-1)
    (-2) edge [bend left]  node        {$  p_{n-2}$} (-3)
    (-3) edge [bend left]  node        {$1-p_{n-1}$} (-2)
    (-3) edge              node        {$  p_{n-1}$} (-4);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

The total number of states the system can be in depends on the number of coin toss events. We can determine the number of states using the following equation:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \ell = 2n + 1
    \\[1em]
    &
    \begin{aligned}
    & \ell && \text{is the number of states the system can be in}
    \\
    & n    && \text{is the number of coin toss events}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The number of states the system can be in determines the size of the state transition matrix. Since this can be an arbitrarily large matrix, it is more practical to describe the contents of the matrix using an algorithm:

{% latex fig-15 %}
    \usepackage[vlined]{algorithm2e}
    \begin{document}
    \begin{algorithm}[H]
    \DontPrintSemicolon
    Let $\mathbf{M}$ be an $\ell \times \ell$ matrix with all elements initalized to zero\;
    \BlankLine
    \BlankLine
    \For{$i = 1$ \KwTo $\ell$}{
        \BlankLine
        $s = i - n - 1$\;
        \BlankLine
        \Switch{$s$}{
            \BlankLine
            \uCase{$|s| = n$}{
                \BlankLine
                break\;
                \BlankLine
            }
            \uCase{$s > 0$}{
                \BlankLine
                $k = |s|$\;
                \BlankLine
                $m_{i,(i + 1)} \leftarrow p_k$\;
                $m_{i,(i - 1)} \leftarrow 1 - p_k$\;
                \BlankLine
            }
            \uCase{$s < 0$}{
                \BlankLine
                $k = |s|$\;
                \BlankLine
                $m_{i,(i + 1)} \leftarrow 1 - p_k$\;
                $m_{i,(i - 1)} \leftarrow p_k$\;
                \BlankLine
            }
            \Case{$s = 0$}{
                \BlankLine
                $m_{i,(i + 1)} \leftarrow p_0$\;
                $m_{i,(i - 1)} \leftarrow p_0$\;
                \BlankLine
            }
        }
    }
    \end{algorithm}
    \end{document}
{% endlatex %}

This is a square matrix with a nonzero value for every possible state transition in the Markov model. We also need to define an initial state vector. Since there is only one initial state, this vector can be described with a very simple algorithm:

{% latex fig-16 %}
    \usepackage[vlined]{algorithm2e}
    \begin{document}
    \begin{algorithm}[H]
    \DontPrintSemicolon
    Let $\mathbf{v}_0$ be a row vector with $\ell$ elements\;
    \BlankLine
    \BlankLine
    \For{$i = 1$ \KwTo $\ell$}{
        \BlankLine
        $s = i - n - 1$\;
        \BlankLine
        \uIf{$s = 0$}{
            \BlankLine
            $v_{0,i} \leftarrow 1$
            \BlankLine
        }
        \Else{
            \BlankLine
            $v_{0,i} \leftarrow 0$
            \BlankLine
        }
    }
    \end{algorithm}
    \end{document}
{% endlatex %}

Once we have a state transition matrix and the initial state vector, we can compute the final outcome using the following formula:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_n = \mathbf{v}_0 \times \mathbf{M}^n
    \end{displaymath}
    \end{document}
{% endlatex %}

The final outcome tells us how likely it is for each state to be the final state of the system after a single round of the coin toss game. We can also represent the final outcome like this:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{v}_n =
    \left[
    \begin{array}{l}
    P(S_{-n})
    \\[1em]
    \multicolumn{1}{c}{\vdots}
    \\[1em]
    P(S_0)
    \\[1em]
    \multicolumn{1}{c}{\vdots}
    \\[1em]
    P(S_{+n})
    \end{array}
    \right]^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Each element contains the probability that the system terminates in the corresponding state after the final coin toss. Since our model is symmetrical about the initial state, there is a symmetry to the resulting values in the final outcome.

## Equality Constraints

Given the model of the coin toss game described above, suppose we know the values of the final outcome but not the values of the weights of the biased coins. Starting with the final outcome---sometimes referred to as the target distribution---we can find a valid set of weights using the method of Lagrange multipliers described in [*Equality Constraints and Lagrange Multipliers*]({% post_url 2020-12-12-equality-constraints-and-lagrange-multipliers %}). To use this method, we need to come up with a set of equality constraints based on the model. Let's start with some equality conditions that must hold true:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = P(S_0)
    \\[1em]
    r_k & = P(S_{-k}) = P(S_{+k}), \quad \forall k \in \{\, 1, \dots, n \,\}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The left-hand side of these equations represents the value of the known target distribution for the corresponding state. The right-hand side represents the computed result based on the values of the weights of the biased coins. These equality conditions are true if we have a valid set of weights. Notice also the symmetry for states above and below the initial state. We can leave out the duplicate conditions because they are redundant. We can also eliminate states that we know are never terminal states. Consider the following:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \mathrlap{R_{\sscr{even}}}\phantom{R_{\sscr{even}}}
    =
    \{\ 0, 2, 4, \dots, n \ \}
    \\
    & \mathrlap{R_{\sscr{odd}}}\phantom{R_{\sscr{even}}}
    =
    \{\ 1, 3, 5, \dots, n \ \}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

These two sets contain even and odd numbers, respectively. We can select one or the other based on whether the number of coin toss events per round is even or odd:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    R =
    \begin{dcases}
    R_{\sscr{even}} & \quad \text{if $n$ is even}
    \\
    R_{\sscr{odd}}  & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using the selected set, we can determine which states are never terminal states. Non-terminal states have a probability of zero in the final outcome. We know the following holds true, no matter what weights are used for the biased coins:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    r_k = 0, \quad \forall k \notin R
    \end{displaymath}
    \end{document}
{% endlatex %}

We can eliminate non-terminal states from consideration because they have no bearing on the equality constraints needed for the method of Lagrange multipliers. The total number of equality constraints then is given by the following:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    m = |R| =
    \begin{dcases}
    \frac{n + 2}{2} & \quad \text{if $n$ is even}
    \\[0.5em]
    \frac{n + 1}{2} & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

The number of equality constraints is a function of the total number of coin toss events. We can establish a set of equality constraints like this:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    f_i(\mathbf{p}) = 0, \quad \forall i \in \{\, 1, \dots, m \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

Each constraint function must be equal to zero. The functions we want to use here are functions that take the difference between the target values and the computed values based on a given set of weights:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    f_i(\mathbf{p}) = r_k - P(S_{\pm k}), \quad
    k =
    \begin{dcases}
    2i - 2 & \quad \text{if $n$ is even}
    \\
    2i - 1 & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using these equality constraints, we can construct a Lagrangian function that can be used to find a valid set of weights:

{% latex fig-26 %}
    \begin{document}
    \begin{displaymath}
    \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    S(\mathbf{p}) - \sum_{i = 1}^{m}{\lambda_i f_i(\mathbf{p})}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can use this Lagrangian function to find a valid set of weights by applying the optimization and root finding methods outlined in previous posts.

## Example with Exponential Distribution

Now let's take a look at an example with ten coin toss events per round. Suppose we start with the following target distribution:

{% chart fig-27-pmfunc-exponent.svg %}

We want to find a valid set of weights that yields this distribution in the final outcome. We'll start with the following initial guess and iteratively work towards a solution:

{% chart fig-28-biases-start-slope.svg %}

We can use the multivariate form of Newton's method to find the weights for which the gradient of the Lagrangian function is equal to zero. This method is described in detail in the post titled [*Finding the Roots with Newton's Method*]({% post_url 2020-12-31-finding-the-roots-with-newtons-method %}). Here we apply the following iterative formula:

{% latex fig-29 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{x}_{i+1}
    =
    \mathbf{x}_i - {\mathbf{J}(\mathbf{x}_i)}^{-1} \mathbf{f}(\mathbf{x}_i)
    \end{displaymath}
    \end{document}
{% endlatex %}

Note the use of the alternative notation scheme above for the gradient of the Lagrangian function. The Lagrangian function we use in this example must have a concrete definition of the scoring function defined. In this example, we use two different scoring functions, defined below.

Here is the definition of scoring function A:

{% latex fig-30 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}(\mathbf{p})
    =
    \sum_{i = 1}^{n - 1}{\big( p_i - 0.5 \big)^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the definition of scoring function B:

{% latex fig-31 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}(\mathbf{p})
    =
    \sum_{i = 1}^{n - 1}{\big( p_i - p_{i-1} \big)^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the solution found using scoring function A:

{% chart fig-32-biases-final-1.svg %}

Here is the solution found using scoring function B:

{% chart fig-33-biases-final-2.svg %}

Here are the number of iterations required for each scoring function:

{% latex fig-34 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \newcommand{\Sa}{\mathrlap{S_a}\phantom{S_b}}
    \newcommand{\Sb}{\mathrlap{S_b}\phantom{S_b}}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{8em}<{$}|>{$}wr{5em}<{$}|}
    \hline
    \text{Scoring Function} & \text{Iterations}
    \\[0.25em]\hline
    \Sa(\mathbf{p})         & \text{5}
    \\[0.25em]\hline
    \Sb(\mathbf{p})         & \text{5}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

In both cases, the initial guess is not too far from the optimal solution. As with previous examples using Newton's method, the system converges in very few iterations. Both solutions are very similar, taking on a sort of zigzag shape.

## Example with Triangular Distribution

Let's take a look at another example with ten coin toss events per round. Suppose we start with the following target distribution:

{% chart fig-35-pmfunc-triangle.svg %}

We want to find a valid set of weights that yields this distribution in the final outcome. We'll start with the following initial guess and iteratively work towards a solution:

{% chart fig-36-biases-start-equal.svg %}

To find a valid set of weights, we'll use the multivariate form of Newton's method like we did in the last example. But this time, we'll include a damping factor to slow down the convergence. Here is the iterative formula:

{% latex fig-37 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{x}_{i+1}
    =
    \mathbf{x}_i - \gamma {\mathbf{J}(\mathbf{x}_i)}^{-1} \mathbf{f}(\mathbf{x}_i)
    \end{displaymath}
    \end{document}
{% endlatex %}

We'll use a damping factor of ten percent:

{% latex fig-38 %}
    \begin{document}
    \begin{displaymath}
    \gamma = 0.1
    \end{displaymath}
    \end{document}
{% endlatex %}

The damping factor slows down the convergence and prevents the method from overshooting. In this particular example, the method fails the converge without slowing down the iterative process. The use of the damping factor would not be necessary if we started with an initial guess closer to the final solution.

Here is the solution found using scoring function A:

{% chart fig-39-biases-final-3.svg %}

Here is the solution found using scoring function B:

{% chart fig-40-biases-final-4.svg %}

Here are the number of iterations required for each scoring function:

{% latex fig-41 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \newcommand{\Sa}{\mathrlap{S_a}\phantom{S_b}}
    \newcommand{\Sb}{\mathrlap{S_b}\phantom{S_b}}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{8em}<{$}|>{$}wr{5em}<{$}|}
    \hline
    \text{Scoring Function} & \text{Iterations}
    \\[0.25em]\hline
    \Sa(\mathbf{p})         & \text{201}
    \\[0.25em]\hline
    \Sb(\mathbf{p})         & \text{188}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

The damped version of Newton's method requires many more iterations to converge than it does with the undamped version used in the previous example. As with the previous example, the two solutions are very similar to one another, this time taking on a slanted shape.

## Shortcomings

The methods used here allow us to find a valid set of weights for a given target distribution using a model of the coin toss game that allows for an arbitrary number of coin toss events. We used the method of Lagrange multiplier to set up an equation, and we used Newton's method to solve the equation. But these methods are not without their shortcomings. As we saw in the previous section, we had to adapt the iterative formula with a damping factor to get the iterative process to converge to a solution.

Besides the overshoot problem, there are cases where these methods might converge to a solution outside the acceptable range of values. The weights of the biased coins must always be a probability between zero and one. There is nothing in the method of Lagrange multipliers that limits values to a particular range. I might have to explore an approach using [Karush--Kuhn--Tucker conditions](https://en.wikipedia.org/wiki/Karush%E2%80%93Kuhn%E2%80%93Tucker_conditions) to include inequality constraints.

Another problem is runtime performance. With the implementation used in this post, finding the solution for a model with ten coin toss events per round takes a couple of seconds to execute on my current hardware. A model with one additional coin toss event per round takes about twice the amount of time to execute. This implementation seems to have an exponential time complexity. There is no parallelism, and I have made no attempt at performance tuning. But I do think there is room for improvement. There might also be other approaches that offer a good enough solution. Ideally, I would like to be able to solve for models with twenty or even fifty flips per round.

{% accompanying_src_link %}
