---
layout: post
title: Equality Constraints and Lagrange Multipliers
---

My last few posts have centered around a weighted coin toss game in which the weights of a set of biased coins are determined based on a known target distribution. And while multiple solutions are possible, the inclusion of a scoring function allowed for a unique solution to be found. Until now, I was not sure how to include the scoring function in such a way that I could solve the problem numerically for an arbitrary number of coin tosses. In this post, I show how to use the [method of Lagrange multipliers](https://en.wikipedia.org/wiki/Lagrange_multiplier) to minimize the scoring function while conforming to the constraints of the coin toss problem.

<!--excerpt-->

## Lagrangian Function

In previous posts, we focused primarily on finding arbitrary solutions to the weighted coin toss problem. The scoring function that ranked each possible solution was a secondary concern. Here we want to focus on minimizing the scoring function as our primary concern while still satisfying the conditions that yield a valid result for a given target distribution. We can frame the problem as a scoring function that we want to minimize along with a set of equality constraints that we need to satisfy:

{% latex 1 fig-01 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Minimize:} & S(\mathbf{p})
    \\[0.4em]
    & \text{Given:}    & f_1(\mathbf{p}) & = 0
    \\
    &                  & f_2(\mathbf{p}) & = 0
    \\
    &                  & \multicolumn{2}{c}{\vdots}
    \\
    &                  & f_m(\mathbf{p}) & = 0
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Our objective is not to find the absolute minimum of the scoring function. Rather, we want to find a set of parameters that satisfy the given constraints while also yielding the smallest possible value when plugged into the scoring function. The key to understanding the method of Lagrange multipliers is knowing that the gradient of the objective function---the scoring function in this case---is equal to a linear combination of the gradients of the constraint functions at the point in which we find the optimal solution:

{% latex 1 fig-02 %}
    \begin{document}
    \begin{displaymath}
    \nabla S(\mathbf{p})
    =
    \sum_{i = 1}^{m} \lambda_i \nabla f_i(\mathbf{p})
    \end{displaymath}
    \end{document}
{% endlatex %}

The lambda coefficients are the Lagrange multipliers. You can find plenty of resources online that explain this relationship in more detail. For our purposes here, let's just take it as a given. If you were to expand out the gradients and include the equality constraints, you would have a system of equations that can be solved. We can package this system of equations up in an elegant fashion using the Lagrangian function:

{% latex 1 fig-03 %}
    \begin{document}
    \begin{displaymath}
    \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    S(\mathbf{p}) - \sum_{i = 1}^{m} \lambda_i f_i(\mathbf{p})
    \end{displaymath}
    \end{document}
{% endlatex %}

Now, if we take the gradient of the Lagrangian function and set it equal to the zero vector, we have a very concise way to express the system of equations:

{% latex 1 fig-04 %}
    \begin{document}
    \begin{displaymath}
    \nabla \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    \mathbf{0}
    \end{displaymath}
    \end{document}
{% endlatex %}

It might even be too concise. But it makes sense if you expand the gradient and unfold it a bit. Here is the complete system of equations:

{% latex 1 fig-05 %}
    \newcommand{\dL}{\pderiv \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})}
    \newcommand{\dS}{\pderiv S(\mathbf{p})}
    \newcommand{\dF}{\pderiv f_i(\mathbf{p})}
    \newcommand{\lhsA}{\dfrac{\dL}{\pderiv p_1}}
    \newcommand{\lhsB}{\dfrac{\dL}{\pderiv p_{n-1}}}
    \newcommand{\lhsC}{\dfrac{\dL}{\pderiv \lambda_1}}
    \newcommand{\lhsD}{\dfrac{\dL}{\pderiv \lambda_m}}
    \newcommand{\rhsA}
    {
        \displaystyle
        \frac{\dS}{\pderiv p_1}
        -
        \sum_{i = 1}^{m} \lambda_i \frac{\dF}{\pderiv p_1}
    }
    \newcommand{\rhsB}
    {
        \displaystyle
        \frac{\dS}{\pderiv p_{n-1}}
        -
        \sum_{i = 1}^{m} \lambda_i \frac{\dF}{\pderiv p_{n-1}}
    }
    \newcommand{\rhsC}{0 - f_1(\mathbf{p})}
    \newcommand{\rhsD}{0 - f_m(\mathbf{p})}
    \begin{document}
    \begin{displaymath}
    \nabla \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    \begin{matrix}{c}
    \vphantom{\rhsA} \lhsA
    \\[1.5em]
    \vdots
    \\[1.5em]
    \vphantom{\rhsB} \lhsB
    \\[2em]
    \lhsC
    \\[1.5em]
    \vdots
    \\[1.5em]
    \lhsD
    \end{matrix}
    =
    \begin{matrix}{c}
    \rhsA
    \\[1.5em]
    \vdots
    \\[1.5em]
    \rhsB
    \\[2em]
    \vphantom{\lhsC} \rhsC
    \\[1.5em]
    \vdots
    \\[1.5em]
    \vphantom{\lhsD} \rhsD
    \end{matrix}
    =
    \mathbf{0}
    \end{displaymath}
    \end{document}
{% endlatex %}

Note that when using the method of Lagrange multipliers, not only do we need to find the weights of the biased coins, but we also need to solve for the Lagrange multipliers. This technique introduces more variables that we need to solve for. But that's no problem. Once we have the complete system of equations, we can get on with the business of solving them.

## Solving with Gradient Descent

There is more than one way to solve a system of equations. One way to do it is to use the gradient descent algorithm demonstrated in my [previous post]({% post_url 2020-09-12-minimizing-with-gradient-descent %}). We can use the magnitude of the gradient as the cost function. In the examples that follow, we'll use the square of the magnitude of the gradient because it's easier that way:

{% latex 1 fig-06 %}
    \begin{document}
    \begin{displaymath}
    C(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    \| \nabla \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup}) \|^2
    \end{displaymath}
    \end{document}
{% endlatex %}

The square of the magnitude can be found by computing the sum of the squares of each element in the gradient vector. The equation above can be expanded out as like this:

{% latex 1 fig-07 %}
    \newcommand{\dL}{\pderiv \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})}
    \begin{document}
    \begin{displaymath}
    C(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    \sum_{i = 1}^{n - 1}
    \2
    \brace3(){\frac{\dL}{\pderiv p_i}}^2
    +
    \sum_{i = 1}^{m}
    \2
    \brace3(){\frac{\dL}{\pderiv \lambda_i}}^2
    \end{displaymath}
    \end{document}
{% endlatex %}

In the examples illustrated in the following sections, we'll use the same learning rate calculation described in my previous post titled [*Minimizing with Gradient Descent*]({% post_url 2020-09-12-minimizing-with-gradient-descent %}). But instead of using a fixed step size, we'll use a variable step size:

{% latex 1 fig-08 %}
    \begin{document}
    \begin{displaymath}
    s_k = \min \2 \brace3{\lbrace}{\rbrace}{\, 0.0001,\, \frac{1}{1 + k} \,}
    \end{displaymath}
    \end{document}
{% endlatex %}

The step size remains constant for the first 10,000 iterations and then decreases asymptotically thereafter. Here is a visualization:

{% chart fig-09-stepsize.svg %}

Decreasing the step size like this helps the algorithm converge to a solution faster. As we will see in the following sections, it still takes a lot of iterations to find a solution using the gradient descent method.

## Example with 3 Coin Tosses

Let's consider a concrete example of using the method of Lagrange multipliers and the gradient descent algorithm to find the optimal solution to the coin toss problem. In this first example, we'll use a model of the coin toss game with three flips per round. You can find a detailed description of this model in my post titled [*Visualizing the Climb up the Hill*]({% post_url 2020-08-16-visualizing-the-climb-up-the-hill %}). Suppose we start with the following target distribution:

{% chart fig-10-target-pmfunc-3.svg %}

This illustration depicts the probability mass function of the expected outcome. These values can be represented using the following notation:

{% latex 1 fig-11 %}
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

Based on the model of the coin toss game with three flips per round, the following constraint functions must equal zero when we have a valid solution for the weights:

{% latex 1 fig-12 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f_1(\mathbf{p})
    & =
    r_1 - 0.5 \1 \brace1(){ 1 - p_1 \0 p_2 }
    \\[1em]
    f_2(\mathbf{p})
    & =
    r_3 - 0.5 \0 p_1 \0 p_2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have the constraint functions, we can go ahead and incorporate them into the Lagrangian function. Here is what it looks like so far:

{% latex 1 fig-13 %}
    \begin{document}
    \begin{displaymath}
    \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    S(\mathbf{p})
    -
    \lambda_1 \1 \brace2[]{ r_1 - 0.5 \1 \brace1(){ 1 - p_1 \0 p_2 } }
    -
    \lambda_2 \1 \brace2[]{ r_3 - 0.5 \0 p_1 \0 p_2 }
    \end{displaymath}
    \end{document}
{% endlatex %}

Now we just need to plug in a scoring function to produce a concrete Lagrangian function that we can work with. The next two sections demonstrate the results of the optimization procedure using two different scoring function variants.

## Example with 3 Coin Tosses, Scoring Function A

Using the two constraint functions defined for the coin toss game with three flips per round, we can construct a Lagrangian function with the following scoring function:

{% latex 1 fig-14 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}(\mathbf{p})
    =
    \brace1(){ p_1 - 0.5 }^2
    +
    \brace1(){ p_2 - 0.5 }^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have a concrete Lagrangian function, we can use it to come up with a cost function as described earlier---by taking the square of the magnitude of the gradient. We can then use this cost function to apply the gradient descent method. The following diagrams show the trace of the gradient descent using four different starting points:

{% chart fig-15-trace-A-1-heatmap.svg %}
{% chart fig-16-trace-A-2-heatmap.svg %}
{% chart fig-17-trace-A-3-heatmap.svg %}
{% chart fig-18-trace-A-4-heatmap.svg %}

Not all dimensions are shown in these illustrations. The colors of the heatmap are based on the final values of the Lagrange multipliers. The important thing to notice here is that, regardless of the starting point, the final value is always the same. And the final value matches the value found by a [different method]({% post_url 2020-08-16-visualizing-the-climb-up-the-hill %}#scoring-function-a).

Here is a breakdown of the number of iterations required for each trace:

{% latex 1 fig-19 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{3em}|wr{5em}|}
    \hline
    \text{Trace} & \text{Iterations}
    \\[0.25em]\hline
    1            & \text{36,357}
    \\[0.25em]\hline
    2            & \text{51,168}
    \\[0.25em]\hline
    3            & \text{36,357}
    \\[0.25em]\hline
    4            & \text{36,333}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using gradient descent to solve the system of equations yields the correct result. But with such a large number of iterations, it doesn't seem like a very efficient way to arrive at the solution. While not visible in the charts, the trace falls into a tight zigzagging pattern as it moves closer to the final solution, slowing down the performance of the algorithm.

## Example with 3 Coin Tosses, Scoring Function B

Using the two constraint functions defined for the coin toss game with three flips per round, we can construct a Lagrangian function with the following scoring function:

{% latex 1 fig-20 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}(\mathbf{p})
    =
    \brace1(){ p_1 - 0.5 }^2
    +
    \brace1(){ p_2 - p_1 }^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have a concrete Lagrangian function, we can use it to come up with a cost function as described earlier---by taking the square of the magnitude of the gradient. We can then use this cost function to apply the gradient descent method. The following diagrams show the trace of the gradient descent using four different starting points:

{% chart fig-21-trace-B-1-heatmap.svg %}
{% chart fig-22-trace-B-2-heatmap.svg %}
{% chart fig-23-trace-B-3-heatmap.svg %}
{% chart fig-24-trace-B-4-heatmap.svg %}

Not all dimensions are shown in these illustrations. The colors of the heatmap are based on the final values of the Lagrange multipliers. The important thing to notice here is that, regardless of the starting point, the final value is always the same. And the final value matches the value found by a [different method]({% post_url 2020-08-16-visualizing-the-climb-up-the-hill %}#scoring-function-b).

Here is a breakdown of the number of iterations required for each trace:

{% latex 1 fig-25 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{3em}|wr{5em}|}
    \hline
    \text{Trace} & \text{Iterations}
    \\[0.25em]\hline
    1            & \text{284,421}
    \\[0.25em]\hline
    2            & \text{284,421}
    \\[0.25em]\hline
    3            & \text{284,421}
    \\[0.25em]\hline
    4            & \text{284,433}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using gradient descent to solve the system of equations yields the correct result. But with such a large number of iterations, it doesn't seem like a very efficient way to arrive at the solution. While not visible in the charts, the trace falls into a tight zigzagging pattern as it moves closer to the final solution, slowing down the performance of the algorithm.

## Example with 4 Coin Tosses

Let's consider another example of using the method of Lagrange multipliers and the gradient descent algorithm to find the optimal solution to the coin toss problem. In this example, we'll use a model of the coin toss game with four flips per round. You can find a detailed description of this model in my post titled [*Estimating the Weights of Biased Coins*]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}). Suppose we start with the following target distribution:

{% chart fig-26-target-pmfunc-4.svg %}

This illustration depicts the probability mass function of the expected outcome. These values can be represented using the following notation:

{% latex 1 fig-27 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.4000
    \\
    r_2 & = 0.2000
    \\
    r_4 & = 0.1000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Based on the model of the coin toss game with four flips per round, the following constraint functions must equal zero when we have a valid solution for the weights:

{% latex 1 fig-28 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f_1(\mathbf{p})
    & =
    r_0 - \brace1(){ 1 - p_1 } \brace1(){ 1 - p_1 \0 p_2 }
    \\[1em]
    f_2(\mathbf{p})
    & =
    r_2 - 0.5 \0 p_1 \brace1(){ 1 + p_2 - p_1 \0 p_2 - p_2 \0 p_3 }
    \\[1em]
    f_3(\mathbf{p})
    & =
    r_4 - 0.5 \0 p_1 \0 p_2 \0 p_3
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can use the constraint functions above to create a Lagrangian function. Using the Lagrangian function, we can come up with a cost function, as described earlier. Once we have a cost function, we can use it to apply the gradient descent algorithm. In this example, we'll start with the following initial guess:

{% latex 1 fig-29 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathbf{p}
    _{\mathrlap{\sscr{start}}\phantom{\sscr{finish}}}
    & =
    \begin{matrix}{l}
    0.5000
    \\[1em]
    0.5000
    \\[1em]
    0.5000
    \end{matrix}
    \\[1em]
    \boldsymbol{\lambdaup}
    _{\mathrlap{\sscr{start}}\phantom{\sscr{finish}}}
    & =
    \begin{matrix}{l}
    0.0000
    \\[1em]
    0.0000
    \\[1em]
    0.0000
    \end{matrix}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now suppose we plug the following scoring function into our Lagrangian function:

{% latex 1 fig-30 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}(\mathbf{p})
    =
    \brace1(){ p_1 - 0.5 }^2
    +
    \brace1(){ p_2 - 0.5 }^2
    +
    \brace1(){ p_3 - 0.5 }^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Applying the gradient descent method, here is the result:

{% latex 1 fig-31 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathbf{p}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \begin{matrix}{l}
    \phantom{+}0.4422
    \\[1em]
    \phantom{+}0.6396
    \\[1em]
    \phantom{+}0.7071
    \end{matrix}
    \\[1em]
    \boldsymbol{\lambdaup}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \begin{matrix}{l}
    +0.0070
    \\[1em]
    +1.4624
    \\[1em]
    -1.4660
    \end{matrix}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now suppose we plug the following scoring function into our Lagrangian function:

{% latex 1 fig-32 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}(\mathbf{p})
    =
    \brace1(){ p_1 - 0.5 }^2
    +
    \brace1(){ p_2 - p_1 }^2
    +
    \brace1(){ p_3 - p_2 }^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Applying the gradient descent method, here is the result:

{% latex 1 fig-33 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathbf{p}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \begin{matrix}{l}
    \phantom{+}0.4487
    \\[1em]
    \phantom{+}0.6116
    \\[1em]
    \phantom{+}0.7288
    \end{matrix}
    \\[1em]
    \boldsymbol{\lambdaup}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \begin{matrix}{l}
    -0.2970
    \\[1em]
    +0.9289
    \\[1em]
    -0.7804
    \end{matrix}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

You can compare these results with the results found via a [different method]({% post_url 2019-11-14-hill-climbing-and-cost-functions %}#scoring-values-on-the-plateau) and see that they are the same. Here are the number of iterations required:

{% latex 1 fig-34 %}
    \newcommand{\Sa}{\mathrlap{S_a}\phantom{S_b}}
    \newcommand{\Sb}{\mathrlap{S_b}\phantom{S_b}}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{8em}|wr{5em}|}
    \hline
    \text{Scoring Function} & \text{Iterations}
    \\[0.25em]\hline
    \Sa(\mathbf{p})         & \text{75,757}
    \\[0.25em]\hline
    \Sb(\mathbf{p})         & \text{448,690}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Once again, we see that the gradient descent algorithm requires a large number of iterations. It's an iterative method that falls into a slow zigzag as it moves closer to the final value.

## Constraint Qualification and Dependent Equations

Using the optimization technique illustrated in the examples above, the solution always converges to the most optimal set of values for the weights of the biased coins, regardless of the initial guess. There is only one unique solution for a given scoring function and set of constraints. When using iterative optimization methods, the optimal point is the solution found upon reaching the final iteration:

{% latex 1 fig-35 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{p}^{*}
    =
    \mathbf{p}_{\sscr{finish}}
    \end{displaymath}
    \end{document}
{% endlatex %}

This technique, however, does not necessarily converge to a unique set of Lagrange multipliers. The Lagrange multiplier theorem guarantees a unique set of Lagrange multipliers only if the constraint qualification assumption is satisfied. Maybe it doesn't matter here, since we still get the right answer for the weights of the biased coins. But I think it's worth pointing out nonetheless. So what is the constraint qualification assumption that guarantees a unique set of Lagrange multipliers? It depends on whether there are multiple constraints or only one constraint. Consider the gradient of the constraint functions at the optimal point:

{% latex 1 fig-36 %}
    \newcommand{\dF}{\pderiv f_i(\mathbf{p}^{*})}
    \begin{document}
    \begin{displaymath}
    \nabla f_i(\mathbf{p}^{*})
    =
    \begin{matrix}{c}
    \dfrac{\dF}{\pderiv p_1}
    \\[1.5em]
    \vdots
    \\[1.5em]
    \dfrac{\dF}{\pderiv p_{n-1}}
    \end{matrix}
    ,
    \quad \forall i \in \{\, 1, \dots, m \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

In the case of multiple constraints, the gradient vectors of each constraint at the optimal point must be linearly independent of each other. Where there is only one constraint, the gradient must not equal the zero vector at the optimal point. None of the examples illustrated in the sections above satisfy the constraint qualification condition. But let's take a closer look at the constraints. Consider the set of constraints for the coin toss game with three flips per round:

{% latex 1 fig-37 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f_1(\mathbf{p})
    & =
    0.3333 - 0.5 \1 \brace1(){ 1 - p_1 \0 p_2 }
    \\[1em]
    f_2(\mathbf{p})
    & =
    0.1667 - 0.5 \0 p_1 \0 p_2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Remember, each constraint function must equal zero. And these two equations are not independent of each other. The second constraint can be expressed in terms of the first:

{% latex 1 fig-38 %}
    \begin{document}
    \begin{displaymath}
    f_2(\mathbf{p}) = -f_1(\mathbf{p})
    \end{displaymath}
    \end{document}
{% endlatex %}

Since it is not an independent equation, we can drop the second constraint entirely. Using only the first constraint, we can apply the method of Lagrange multipliers in a way that satisfies the constraint qualification assumption. In this case, there is only one Lagrange multiplier, and it always converges to the same value using iterative methods, regardless of the initial guess. Now let's consider the set of constraints for the coin toss game with four flips per round:

{% latex 1 fig-39 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f_1(\mathbf{p})
    & =
    0.4000 - \brace1(){ 1 - p_1 } \brace1(){ 1 - p_1 \0 p_2 }
    \\[1em]
    f_2(\mathbf{p})
    & =
    0.2000 - 0.5 \0 p_1 \brace1(){ 1 + p_2 - p_1 \0 p_2 - p_2 \0 p_3 }
    \\[1em]
    f_3(\mathbf{p})
    & =
    0.1000 - 0.5 \0 p_1 \0 p_2 \0 p_3
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Again, each constraint function must equal zero. And like before, this is not an independent set of equations. We can express one constraint as a linear combination of the others:

{% latex 1 fig-40 %}
    \begin{document}
    \begin{displaymath}
    f_2(\mathbf{p}) = -0.5 f_1(\mathbf{p}) - f_3(\mathbf{p})
    \end{displaymath}
    \end{document}
{% endlatex %}

If we exclude the dependent equation from the set of constraints, we can satisfy the constraint qualification assumption. As I mentioned earlier, satisfying the constraint qualification assumption may not be necessary if we only care about the optimal set of values for the weights of the biased coins. But this knowledge might come in handy when exploring other optimization techniques that might be more efficient than the gradient descent method used in the examples illustrated in the previous sections.

## Other Optimization and Root Finding Methods

In the examples above, the gradient descent method requires tens of thousands of iterations to converge to a solution. In some cases, it even takes hundreds of thousands of iterations. While I tried to optimize it a bit using a variable step size, it still seems like a large number of iterations. The [numerics library](https://numerics.mathdotnet.com/) I am using comes with some alternative optimization methods that can be used in place of my gradient descent implementation. These alternatives can find the minimum of the cost function in far fewer iterations. See the tables below.

Example with 3 coin tosses, scoring function A:

{% latex 1 fig-41 %}
    \newcommand{\methodA}{\text{Gradient descent}}
    \newcommand{\methodB}{\text{Nelder--Mead}}
    \newcommand{\methodC}{\text{Broyden--Fletcher--Goldfarb--Shanno}}
    \newcommand{\methodD}{\text{Newton--Raphson}}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{16em}|wr{5em}|}
    \hline
    \text{Method} & \text{Iterations}
    \\[0.25em]\hline
    \methodA      & \text{33,243}
    \\[0.25em]\hline
    \methodB      & \text{220}
    \\[0.25em]\hline
    \methodC      & \text{14}
    \\[0.25em]\hline
    \methodD      & \text{6}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Example with 3 coin tosses, scoring function B:

{% latex 1 fig-42 %}
    \newcommand{\methodA}{\text{Gradient descent}}
    \newcommand{\methodB}{\text{Nelder--Mead}}
    \newcommand{\methodC}{\text{Broyden--Fletcher--Goldfarb--Shanno}}
    \newcommand{\methodD}{\text{Newton--Raphson}}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{16em}|wr{5em}|}
    \hline
    \text{Method} & \text{Iterations}
    \\[0.25em]\hline
    \methodA      & \text{284,227}
    \\[0.25em]\hline
    \methodB      & \text{233}
    \\[0.25em]\hline
    \methodC      & \text{11}
    \\[0.25em]\hline
    \methodD      & \text{5}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Example with 4 coin tosses, scoring function A:

{% latex 1 fig-43 %}
    \newcommand{\methodA}{\text{Gradient descent}}
    \newcommand{\methodB}{\text{Nelder--Mead}}
    \newcommand{\methodC}{\text{Broyden--Fletcher--Goldfarb--Shanno}}
    \newcommand{\methodD}{\text{Newton--Raphson}}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{16em}|wr{5em}|}
    \hline
    \text{Method} & \text{Iterations}
    \\[0.25em]\hline
    \methodA      & \text{72,805}
    \\[0.25em]\hline
    \methodB      & \text{597}
    \\[0.25em]\hline
    \methodC      & \text{32}
    \\[0.25em]\hline
    \methodD      & \text{14}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Example with 4 coin tosses, scoring function B:

{% latex 1 fig-44 %}
    \newcommand{\methodA}{\text{Gradient descent}}
    \newcommand{\methodB}{\text{Nelder--Mead}}
    \newcommand{\methodC}{\text{Broyden--Fletcher--Goldfarb--Shanno}}
    \newcommand{\methodD}{\text{Newton--Raphson}}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{16em}|wr{5em}|}
    \hline
    \text{Method} & \text{Iterations}
    \\[0.25em]\hline
    \methodA      & \text{448,288}
    \\[0.25em]\hline
    \methodB      & \text{546}
    \\[0.25em]\hline
    \methodC      & \text{24}
    \\[0.25em]\hline
    \methodD      & \text{9}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

While one method might be able to arrive at a solution in fewer iterations than another method, it is important to remember that not all iterations are created equally. An iteration in one method might be computationally more expensive than an iteration in another. When taking runtime performance into consideration, the best method might not be the one with the fewest iterations. Also, finding the minimum of a cost function is not the only way to solve for the unknowns. Keep in mind, the ultimate goal is to find the parameters in which the gradient of the Lagrangian function is equal to the zero vector:

{% latex 1 fig-45 %}
    \begin{document}
    \begin{displaymath}
    \nabla \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup})
    =
    \mathbf{0}
    \end{displaymath}
    \end{document}
{% endlatex %}

The critical points of the Lagrangian function may occur at saddle points instead of at minimums or maximums. There are some root finding methods that can solve this type of problem directly without having to use a cost function. With the numerics library I am using, I was able to apply Broyden's method to solve the coin toss problem based on the gradient of the Lagrangian function, without having to use a cost function.

{% accompanying_src_link %}
