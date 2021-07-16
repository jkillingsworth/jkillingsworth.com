---
layout: post
title: Visualizing the Climb up the Hill
---

The hill climbing algorithm described in my [previous post]({% post_url 2019-11-14-hill-climbing-and-cost-functions %}) finds the weights of biased coins for a coin toss game in which the distribution of possible outcomes is known. In the example presented, there are many possible solutions. A cost function is used to find a valid solution, and a scoring function is used to narrow down the set of valid solutions to a single result. In this post, I want to look at some visualizations to get a better feel for how the algorithm works.

<!--excerpt-->

## The Model

We'll use the same model of the coin toss game described in an earlier post titled [*Estimating the Weights of Biased Coins*]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}). But to be able to plot the value of the cost function for every combination of weights on a two dimensional illustration, we need to limit the number of coin tosses in each round so that there are only two different weights that can vary. Since the first coin toss always uses a fairly weighted coin, we can model the coin toss game using three flips per round. The Markov model looks like this:

{% latex fig-01 %}
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

In this model, every round starts in the zero state. The coin in the zero state is always a fair coin. The model can be represented in tabular form like this:

{% latex fig-02 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{5em}<{$}|>{$}wl{9em}<{$}|>{$}wl{9em}<{$}|}
    \hline
    \text{State} & \text{Probability of Heads} & \text{Probability of Tails}
    \\[0.25em]\hline
    +2 & p_2 & 1 - p_2
    \\[0.25em]\hline
    +1 & p_1 & 1 - p_1
    \\[0.25em]\hline
    \phantom{+}0 \text{ (start)} & p_0 = 0.5 & p_0 = 0.5
    \\[0.25em]\hline
    -1 & 1 - p_1 & p_1
    \\[0.25em]\hline
    -2 & 1 - p_2 & p_2
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since there are three coin tosses per round, with two possible outcomes for each flip, there are a total of eight unique coin toss sequences possible. The following table lists all possible outcomes along with the terminal state and probability formula for each combination:

{% latex fig-03 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{5em}<{$}|>{$}wl{7em}<{$}|>{$}wl{11em}<{$}|}
    \hline
    \text{Sequence} & \text{Terminal State} & \text{Probability}
    \\[0.25em]\hline
    \texttt{HHH} & +3 & p_0 \, p_1 \, p_2
    \\[0.25em]\hline
    \texttt{HHT} & +1 & p_0 \, p_1 \, (1 - p_2)
    \\[0.25em]\hline
    \texttt{HTH} & +1 & p_0 \, (1 - p_1) \, p_0
    \\[0.25em]\hline
    \texttt{THH} & +1 & p_0 \, (1 - p_1) \, p_0
    \\[0.25em]\hline
    \texttt{HTT} & -1 & p_0 \, (1 - p_1) \, p_0
    \\[0.25em]\hline
    \texttt{THT} & -1 & p_0 \, (1 - p_1) \, p_0
    \\[0.25em]\hline
    \texttt{TTH} & -1 & p_0 \, p_1 \, (1 - p_2)
    \\[0.25em]\hline
    \texttt{TTT} & -3 & p_0 \, p_1 \, p_2
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

While the model has a total of seven states, there are only four possible terminal states. And since the model is symmetrical, the probability of ending on one of the positive terminal states is equal to the probability of ending on the corresponding negative terminal state:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = P(S_{-1}) = P(S_{+1})
    \\[1em]
    r_3 & = P(S_{-3}) = P(S_{+3})
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging in the probability formulas, the equations above can be rewritten as follows:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = 2\,\Big[\, p_0 \, (1 - p_1) \, p_0 \,\Big]
          +    \Big[\, p_0 \, p_1 \, (1 - p_2) \,\Big]
    \\[1em]
    r_3 & = \phantom{\Big[}\, p_0 \, p_1 \, p_2 \,\phantom{\Big]}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Simplifying, these equations can be reduced to the following:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = 0.5\,\big( 1 - p_1\,p_2 \big)
    \\[1em]
    r_3 & = 0.5\,p_1\,p_2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Based on the set of equations above, we can state a relationship between the weight of the coin in the +1 state and the weight of the coin in the +2 state:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    p_2 = \frac{1 - 2 r_1}{p_1}
    \end{displaymath}
    \end{document}
{% endlatex %}

The bias of each coin must be between a minimum value of zero and a maximum value of one. If we know the expected outcome of the coin toss game, the minimum and maximum values might be further constrained. Suppose the coin in the +2 state always lands on heads:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{p_{1,\sscr{min}}}\phantom{p_{1,\sscr{max}}} & = 1 - 2 r_1
    \\[1em]
    \mathrlap{p_{2,\sscr{max}}}\phantom{p_{1,\sscr{max}}} & = 1
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The case above shows the minimum value for the weight of the coin in the +1 state when the coin in the +2 state is at the maximum. The opposite extreme occurs when the coin in the +1 state always lands on heads:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{p_{1,\sscr{max}}}\phantom{p_{1,\sscr{max}}} & = 1
    \\[1em]
    \mathrlap{p_{2,\sscr{min}}}\phantom{p_{1,\sscr{max}}} & = 1 - 2 r_1
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

As demonstrated in my two previous posts, there is a range of valid values that are possible for a given target distribution. There is no unique solution without including an additional constraint such as a scoring function.

## Cost Function

A cost function assigns a value to a set of weights based on a target distribution. The value of the cost function determines how close an estimated set of weights is to a valid set of weights for a given probability mass function. Suppose the target distribution looks like this:

{% chart fig-10-target-pmfunc.svg %}

The probability mass function illustrated above is symmetrical. The values can also be represented like this:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = 0.3333
    \\
    r_3 & = 0.1667
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Our cost function must compare the difference between the target distribution and the expected outcome of a given set of weights. In this example, as with the previous post, I want to use the sum of squared errors as the cost function:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    C(p_1, p_2)
    =
    \Big[\, r_1 - 0.5\,\big( 1 - p_1\,p_2 \big) \,\Big]^2
    +
    \Big[\, r_3 - 0.5\,p_1\,p_2 \,\Big]^2
    \end{displaymath}
    \end{document}
{% endlatex %}

A value of zero is the most ideal. A zero value means the given weights will have an expected outcome that matches the target distribution. A non-zero value indicates an error relative to the ideal. Here is what the cost function looks like on a three-dimensional surface plot:

{% chart fig-13-surface-1.svg %}

This gives a visualization of where the high and low spots are. Since the cost function is used in the context of a hill climbing algorithm, it might be more intuitive to display the values on an inverted surface plot because it makes it look more like a hill:

{% chart fig-14-surface-2.svg %}

A surface plot does a nice job of illustrating the curvature of the cost function. For overlaying additional information in top of the cost function, a heatmap might be a more useful alternative. Here is a visualization of the cost function as a heatmap:

{% chart fig-15-heatmap-plateau.svg %}

In the chart above, the plateau of valid values is superimposed on top of the heatmap as a green line. These are inputs to the cost function that will yield a zero value. As you can see, there is a range of solutions that will yield an expected outcome that matches the target distribution.

## Steepest Ascent Hill Climb

Given a target distribution, the steepest ascent hill climbing algorithm can be used to find a valid set of weights starting with an arbitrary initial guess. The steepest ascent method always chooses the most optimal move for every iteration until the cost function is minimized. My [previous post]({% post_url 2019-11-14-hill-climbing-and-cost-functions %}) describes this algorithm in depth. Let's consider an initial guess that looks like this:

{% chart fig-16-trace-calculated-1-begin-biases.svg %}

Starting with the initial guess above, the algorithm converges on the following set of values:

{% chart fig-17-trace-calculated-1-final-biases.svg %}

The algorithm terminates after 6,047 iterations. The following trace shows the path it takes from start to finish:

{% chart fig-18-trace-calculated-1-heatmap.svg %}

Using the same technique, we can run through several more examples, each with different starting points:

{% chart fig-19-trace-calculated-2-heatmap.svg %}
{% chart fig-20-trace-calculated-3-heatmap.svg %}
{% chart fig-21-trace-calculated-4-heatmap.svg %}

In each example above, the path follows a vertical, horizontal, or diagonal route towards an optimum set of values. Sometimes the route bends in a dog-leg fashion part of the way through. The final set of weights discovered by the hill climbing algorithm differs based on the starting position of the initial guess.

## Stochastic Hill Climb

Unlike the steepest ascent hill climbing algorithm used in the previous section, a stochastic hill climbing algorithm does not always choose the best move on each iteration. Instead, it randomly chooses from any move that improves the cost function. In this example, the random decision is weighted based on how much each potential move improves the cost function. Consider the following initial guess:

{% chart fig-22-trace-stochastic-1-begin-biases.svg %}

Starting with the initial guess above, the algorithm converges on the following set of values:

{% chart fig-23-trace-stochastic-1-final-biases.svg %}

The algorithm terminates after 6,070 iterations. The following trace shows the path it takes from start to finish:

{% chart fig-24-trace-stochastic-1-heatmap.svg %}

Using the same technique, we can run through several more examples, each with different starting points:

{% chart fig-25-trace-stochastic-2-heatmap.svg %}
{% chart fig-26-trace-stochastic-3-heatmap.svg %}
{% chart fig-27-trace-stochastic-4-heatmap.svg %}

Again, the final set of weights discovered by the hill climbing algorithm varies based on the starting position. But the final results are not the same as those found by the steepest ascent algorithm. In these examples, the trace follows a fairly direct route to the final set of values. Although slightly curved, the routes do not make any sharp changes in direction.

## Scoring Function A

A scoring function can be used in addition to a cost function to constrain the valid set of weights to a single optimum value. One scoring function analyzed in my previous post computes the sum of squared differences between the weights and the centermost value. Here is the function:

{% latex fig-28 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}(p_1, p_2)
    =
    \big( p_1 - 0.5 \big)^2 + \big( p_2 - 0.5 \big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Keep in mind we only want to consider inputs in which the cost function evaluates to zero. We can use a numerical method to find the optimum parameter values that minimize the scoring function when the cost function is zero:

{% latex fig-29 %}
    \begin{document}
    \begin{displaymath}
    p_{1,\sscr{optimum}}
    =
    \operatorname*{argmin}_{p_1} \big\{\, \mathrlap{S_a}\phantom{S_b} \,|\, C = 0 \,\big\}
    \approx 0.5773
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a visual representation of the minimization of the scoring function:

{% chart fig-30-score-A-scores-overall.svg %}

And here is the optimum set of weights for this scoring function:

{% chart fig-31-score-A-biases-optimum.svg %}

Since the scoring function can compute a score for all combinations of weights, the scoring function can be represented on a surface plot or heatmap in much the same way as the cost function. Here is a visualization of the scoring function as a heatmap:

{% chart fig-32-score-A-heatmap-S.svg %}

Notice the optimum value does not fall on the point with the best absolute score. This is because it is constrained to values where the cost function is zero. Here is the same depiction overlaid on a heatmap of the cost function:

{% chart fig-33-score-A-heatmap-C.svg %}

In the two illustrations above, the intensity of the plateau line varies based on the score. A better score is depicted with a thicker and more brightly colored line.

## Scoring Function B

The scoring function in the previous section is not the only one possible. Another scoring function analyzed in my previous post computes the sum of squared differences between each weight and the weight of its preceding neighbor. Here is the function:

{% latex fig-34 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}(p_1, p_2)
    =
    \big( p_1 - 0.5 \big)^2 + \big( p_2 - p_1 \big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Keep in mind we only want to consider inputs in which the cost function evaluates to zero. We can use a numerical method to find the optimum parameter values that minimize the scoring function when the cost function is zero:

{% latex fig-35 %}
    \begin{document}
    \begin{displaymath}
    p_{1,\sscr{optimum}}
    =
    \operatorname*{argmin}_{p_1} \big\{\, \mathrlap{S_b}\phantom{S_b} \,|\, C = 0 \,\big\}
    \approx 0.5624
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a visual representation of the minimization of the scoring function:

{% chart fig-36-score-B-scores-overall.svg %}

And here is the optimum set of weights for this scoring function:

{% chart fig-37-score-B-biases-optimum.svg %}

Since the scoring function can compute a score for all combinations of weights, the scoring function can be represented on a surface plot or heatmap in much the same way as the cost function. Here is a visualization of the scoring function as a heatmap:

{% chart fig-38-score-B-heatmap-S.svg %}

Notice the optimum value does not fall on the point with the best absolute score. This is because it is constrained to values where the cost function is zero. Here is the same depiction overlaid on a heatmap of the cost function:

{% chart fig-39-score-B-heatmap-C.svg %}

In the two illustrations above, the intensity of the plateau line varies based on the score. A better score is depicted with a thicker and more brightly colored line.

## Next Steps

Simplifying the model to three coin tosses per round makes it possible to visualize a cost function, scoring function, and path taken by a hill climbing algorithm. It also makes the model easier to reason about. But I still haven't come up with a general solution to finding the optimum set of weights for a given cost function and scoring function in a model with an arbitrarily large number of coin tosses per round. I still have some ideas I want to explore. I am hoping these visualizations can help aid further analysis.

{% accompanying_src_link %}
