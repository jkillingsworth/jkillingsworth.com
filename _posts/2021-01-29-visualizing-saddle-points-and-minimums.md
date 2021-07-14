---
layout: post
title: Visualizing Saddle Points and Minimums
---

The two previous posts demonstrated how to use the method of Lagrange multipliers to find the optimum solution for a coin toss game with biased coins of unknown weight. In one case, we found the minimum of a cost function based on the Lagrangian function. In the other case, we found the saddle point of the Lagrangian function itself. The purpose of this post is to provide some visual representations of these functions.

<!--excerpt-->

## The Model

The model of the coin toss game we'll use here is a simplified version. It uses only two flips per round instead of three flips or four flips per round as used in previous examples. Here is what the Markov model looks like:

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
    (00) edge              node [swap] {$0.5$} (+1)
    (+1) edge [bend right] node [swap] {$1-p$} (00)
    (+1) edge              node        {$  p$} (+2)
    (00) edge              node        {$0.5$} (-1)
    (-1) edge [bend left]  node        {$1-p$} (00)
    (-1) edge              node        {$  p$} (-2);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

The first coin toss is always a fair coin, by definition. Let's assume the probability of tossing both a heads and a tails, in any order, is twice that of getting either two heads in a row or two tails in a row. Let's also assume a scoring function that gives preference to weights that are as close to that of a fair coin as possible. The Lagrangian function for this model looks like this:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \mathcal{L}(p, \lambda)
    =
    \Big( p - 0.5 \Big)^2 - \lambda \Big( 0.5 - p \Big)
    \end{displaymath}
    \end{document}
{% endlatex %}

Like we did previously, we can derive a cost function from the Lagrangian function by computing the magnitude of the gradient of the Lagrangian function. We'll take the square of the magnitude to simplify the calculation. Here is the cost function:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    C(p, \lambda)
    =
    \Big( 2p - 1 + \lambda \Big)^2 + \Big( p - 0.5 \Big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

For such a simple model, it might be obvious what the solution is. The critical points for both functions lie at the same location. But for the Lagrangian function, the critical point exists at a saddle point, whereas the critical point for the cost function lies at the minimum. The illustrations in the following sections provide the visual representations.

## Lagrangian Function

The optimum value lies at the point at which the gradient is equal to the zero vector:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \nabla \mathcal{L}(p, \lambda)
    =
    \mathbf{0}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a heatmap of the Lagrangian function:

{% chart fig-05-lagrange-heatmap-0.svg %}

Here is a surface plot of the function at three slightly different angles:

{% chart fig-06-lagrange-surface-1.svg %}
{% chart fig-07-lagrange-surface-2.svg %}
{% chart fig-08-lagrange-surface-3.svg %}

Here is the profile of a slice of the function taken across a diagonal line:

{% chart fig-09-lagrange-heatmap-1.svg %}
{% chart fig-10-lagrange-profile-1.svg %}

Here is the profile of another slice taken across a different diagonal:

{% chart fig-11-lagrange-heatmap-2.svg %}
{% chart fig-12-lagrange-profile-2.svg %}

The surface plot resembles the shape of a cowboy hat or a western horse saddle. The left and right sides are tilted upwards, while the front and back sides are tilted downwards. In the profile charts, one is convex while the other is concave. This type of curvature is characteristic of a function with a saddle point.

## Cost Function

The optimum value lies at the point at which the gradient is equal to the zero vector:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    \nabla C(p, \lambda)
    =
    \mathbf{0}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a heatmap of the cost function:

{% chart fig-14-costfunc-heatmap-0.svg %}

Here is a surface plot of the function at three slightly different angles:

{% chart fig-15-costfunc-surface-1.svg %}
{% chart fig-16-costfunc-surface-2.svg %}
{% chart fig-17-costfunc-surface-3.svg %}

Here is the profile of a slice of the function taken across a diagonal line:

{% chart fig-18-costfunc-heatmap-1.svg %}
{% chart fig-19-costfunc-profile-1.svg %}

Here is the profile of another slice taken across a different diagonal:

{% chart fig-20-costfunc-heatmap-2.svg %}
{% chart fig-21-costfunc-profile-2.svg %}

The surface plot resembles the shape of an elongated bowl or an enclosed valley. All sides are tilted upwards. In the profile charts, both profiles show a convex curve. As a matter of fact, all slices through the optimum point will be convex. This type of curvature is characteristic of a function with a minimum point.

{% accompanying_src_link %}
