---
layout: post
title: Finding the Roots with Newton's Method
---

In the [last post]({% post_url 2020-12-12-equality-constraints-and-lagrange-multipliers %}), we explored the use of gradient descent and other optimization methods to find the root of a Lagrangian function. These optimization methods work by finding the minimum of a cost function. In this post, I want to explore the multivariate form of Newton's method as an alternative. Unlike optimization methods such as gradient descent, Newton's method can find solutions that lie on a saddle point, eliminating the need for a cost function. This may or may not be a better approach.

<!--excerpt-->

## The Method

Newton's method is an iterative root finding technique for solving equations. It is sometimes called the Newton--Raphson method. In the univariate form, you can use this method to find the root of an equation in the following form:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    f(x) = 0
    \end{displaymath}
    \end{document}
{% endlatex %}

Starting with an initial guess, you can iteratively find successively closer and closer approximations to the root. If you've taken an undergraduate course in numerical methods, the following formula might look familiar:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    x_{i+1} = x_i - \dfrac{f(x_i)}{f'(x_i)}
    \end{displaymath}
    \end{document}
{% endlatex %}

You can apply the formula above repeatedly until you find a sufficiently close value. This method works for a single equation and a single unknown. But what happens if you have multiple equations and multiple unknowns? Consider the following:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f_1(x_1, x_2, \dots, x_n) & = 0
    \\
    f_2(x_1, x_2, \dots, x_n) & = 0
    \\
    \vdots
    \\
    f_m(x_1, x_2, \dots, x_n) & = 0
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is a system of equations that must be satisfied. And each equation has multiple unknowns. We can represent this more concisely using a vector function:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{f}(\mathbf{x})
    =
    \mathbf{0}
    \end{displaymath}
    \end{document}
{% endlatex %}

The multivariate form of Newton's method works in very much the same way as the univariate form. The main difference is, the initial guess and the revised estimate are vectors instead of scalar values. Here is the formula:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{x}_{i+1} = \mathbf{x}_i - {\mathbf{J}(\mathbf{x}_i)}^{-1} \mathbf{f}(\mathbf{x}_i)
    \end{displaymath}
    \end{document}
{% endlatex %}

Notice that instead of dividing the function by its derivative like we do in the univariate case, here we multiply the function by the inverse of its Jacobian matrix. I like to think of the Jacobian matrix as the multivariate equivalent of a derivative. The Jacobian matrix of a vector function is a matrix containing all of its partial derivatives:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{J}(\mathbf{x})
    =
    \frac{d \mathbf{f}(\mathbf{x})}{d \mathbf{x}}
    =
    \left[
    \begin{array}{cccc}
    \dfrac{\partial f_1(\mathbf{x})}{\partial x_1}
    &
    \dfrac{\partial f_1(\mathbf{x})}{\partial x_2}
    &
    \cdots
    &
    \dfrac{\partial f_1(\mathbf{x})}{\partial x_n}
    \\[2em]
    \dfrac{\partial f_2(\mathbf{x})}{\partial x_1}
    &
    \dfrac{\partial f_2(\mathbf{x})}{\partial x_2}
    &
    \cdots
    &
    \dfrac{\partial f_2(\mathbf{x})}{\partial x_n}
    \\[2em]
    \vdots
    &
    \vdots
    &
    \ddots
    &
    \vdots
    \\[2em]
    \dfrac{\partial f_m(\mathbf{x})}{\partial x_1}
    &
    \dfrac{\partial f_m(\mathbf{x})}{\partial x_2}
    &
    \cdots
    &
    \dfrac{\partial f_m(\mathbf{x})}{\partial x_n}
    \end{array}
    \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

Since we are interested in the inverse of the Jacobian matrix, we need to consider what happens if the matrix is not invertible. A matrix is not invertible if it is not a square matrix. Our Jacobian might be a rectangular matrix. And even if it's a square matrix, it might be a singular matrix with a zero determinant. This can happen if one of the elements of the vector function is a linear combination of the others. To get around these problems, we can use a [Moore--Penrose pseudoinverse](https://en.wikipedia.org/wiki/Moore%E2%80%93Penrose_inverse) instead. There are a number of ways to compute the pseudoinverse. One method is to use [singular value decomposition](https://en.wikipedia.org/wiki/Singular_value_decomposition). The matrix can be factorized as follows:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{J} = \mathbf{U} \mathbf{\Sigma} \mathbf{V}^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the singular value decomposition of the Jacobian matrix. It is broken down into the product of three distinct matrices. The individual components can then be recomposed in the following form to find the pseudoinverse:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{J}^{-1} = \mathbf{V} \mathbf{\Sigma}^{-1} \mathbf{U}^{\sscr{T}}
    \end{displaymath}
    \end{document}
{% endlatex %}

The specific details regarding singular value decomposition are beyond the scope of this post. You can find plenty of resources online if you want to learn more about it. In the examples in the following sections, I am using a [numerics library](https://numerics.mathdotnet.com/Matrix.html) to compute the pseudoinverse. The library uses singular value decomposition under the hood.

## Solving the Coin Toss Problem

In the [previous post]({% post_url 2020-12-12-equality-constraints-and-lagrange-multipliers %}), we created a Lagrangian function based on the model of a weighted coin toss game. We then used the gradient descent algorithm to find the point at which the gradient of the Lagrangian function is equal to zero. Now we want to use Newton's method instead of gradient descent to solve the coin toss problem. The generic notation used to describe Newton's method in the previous section is different than the notation used for the model of the coin toss game. Here is a mapping of the two different notation schemes:

{% latex fig-09 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{8em}<{$}|>{$}wl{12em}<{$}|}
    \hline
    \text{Generic Notation} & \text{Coin Toss Game Notation}
    \\[0.25em]\hline
    \mathbf{f}(\mathbf{x}) = \mathbf{0}
    &
    \nabla \mathcal{L}(\mathbf{p}, \boldsymbol{\lambdaup}) = \mathbf{0}
    \\[0.25em]\hline
    n
    &
    n + m - 1
    \\[0.25em]\hline
    m
    &
    n + m - 1
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

We will use the coin toss game notation in the following sections. The examples shown in the following sections reproduce the solutions to the coin toss problem presented in the previous post. Only this time around, we use Newton's method instead of gradient descent to find the zeros of the gradient of the Lagrangian function for each example.

## Example with 3 Coin Tosses, Scoring Function A

Using the same target distribution and constraint functions as we did with the [example in the previous post]({% post_url 2020-12-12-equality-constraints-and-lagrange-multipliers %}#example-with-3-coin-tosses-scoring-function-a), let's use Newton's method to find the solution to this variation of the coin toss problem. We can construct a Lagrangian function with the following scoring function:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}(\mathbf{p})
    =
    \big( p_1 - 0.5 \big)^2 + \big( p_2 - 0.5 \big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have the Lagrangian function, we can compute the gradient and use it to apply Newton's method. The following diagrams show the trace of each iteration of Newton's method using four different starting points:

{% chart fig-11-trace-A-1-heatmap.svg %}
{% chart fig-12-trace-A-2-heatmap.svg %}
{% chart fig-13-trace-A-3-heatmap.svg %}
{% chart fig-14-trace-A-4-heatmap.svg %}

Regardless of the starting point, the final value is always the same. This is what we expect. And this matches the solution found via other methods. Now check out the number of iterations required for each trace:

{% latex fig-15 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{3em}<{$}|>{$}wr{5em}<{$}|}
    \hline
    \text{Trace} & \text{Iterations}
    \\[0.25em]\hline
    1            & \text{5}
    \\[0.25em]\hline
    2            & \text{4}
    \\[0.25em]\hline
    3            & \text{5}
    \\[0.25em]\hline
    4            & \text{5}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Compare that to the number of iterations required using the gradient descent method. Newton's method doesn't require nearly as many iterations as gradient descent.

## Example with 3 Coin Tosses, Scoring Function B

Using the same target distribution and constraint functions as we did with the [example in the previous post]({% post_url 2020-12-12-equality-constraints-and-lagrange-multipliers %}#example-with-3-coin-tosses-scoring-function-b), let's use Newton's method to find the solution to this variation of the coin toss problem. We can construct a Lagrangian function with the following scoring function:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}(\mathbf{p})
    =
    \big( p_1 - 0.5 \big)^2 + \big( p_2 - p_1 \big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have the Lagrangian function, we can compute the gradient and use it to apply Newton's method. The following diagrams show the trace of each iteration of Newton's method using four different starting points:

{% chart fig-17-trace-B-1-heatmap.svg %}
{% chart fig-18-trace-B-2-heatmap.svg %}
{% chart fig-19-trace-B-3-heatmap.svg %}
{% chart fig-20-trace-B-4-heatmap.svg %}

Regardless of the starting point, the final value is always the same. This is what we expect. And this matches the solution found via other methods. Now check out the number of iterations required for each trace:

{% latex fig-21 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{3em}<{$}|>{$}wr{5em}<{$}|}
    \hline
    \text{Trace} & \text{Iterations}
    \\[0.25em]\hline
    1            & \text{5}
    \\[0.25em]\hline
    2            & \text{4}
    \\[0.25em]\hline
    3            & \text{5}
    \\[0.25em]\hline
    4            & \text{5}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Compare that to the number of iterations required using the gradient descent method. Newton's method doesn't require nearly as many iterations as gradient descent.

## Example with 4 Coin Tosses

Let's consider another example using a model of the coin toss game with four flips per round. Like the examples above, we'll use the same target distribution and constraint functions as we did with the [example in the previous post]({% post_url 2020-12-12-equality-constraints-and-lagrange-multipliers %}#example-with-4-coin-tosses). Suppose we start with the following initial guess:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathbf{p}
    _{\mathrlap{\sscr{start}}\phantom{\sscr{finish}}}
    & =
    \left[
    \begin{array}{l}
    0.5000
    \\[1em]
    0.5000
    \\[1em]
    0.5000
    \end{array}
    \right]
    \\[1em]
    \boldsymbol{\lambdaup}
    _{\mathrlap{\sscr{start}}\phantom{\sscr{finish}}}
    & =
    \left[
    \begin{array}{l}
    0.0000
    \\[1em]
    0.0000
    \\[1em]
    0.0000
    \end{array}
    \right]
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now suppose we plug the following scoring function into our Lagrangian function:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}(\mathbf{p})
    =
    \big( p_1 - 0.5 \big)^2 + \big( p_2 - 0.5 \big)^2 + \big( p_3 - 0.5 \big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Applying Newton's method, here is the result:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathbf{p}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \left[
    \begin{array}{l}
    \phantom{+}0.4422
    \\[1em]
    \phantom{+}0.6396
    \\[1em]
    \phantom{+}0.7071
    \end{array}
    \right]
    \\[1em]
    \boldsymbol{\lambdaup}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \left[
    \begin{array}{l}
    +0.0070
    \\[1em]
    +1.4624
    \\[1em]
    -1.4660
    \end{array}
    \right]
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now suppose we plug the following scoring function into our Lagrangian function:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}(\mathbf{p})
    =
    \big( p_1 - 0.5 \big)^2 + \big( p_2 - p_1 \big)^2 + \big( p_3 - p_2 \big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Applying Newton's method, here is the result:

{% latex fig-26 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathbf{p}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \left[
    \begin{array}{l}
    \phantom{+}0.4487
    \\[1em]
    \phantom{+}0.6116
    \\[1em]
    \phantom{+}0.7288
    \end{array}
    \right]
    \\[1em]
    \boldsymbol{\lambdaup}
    _{\mathrlap{\sscr{finish}}\phantom{\sscr{finish}}}
    & =
    \left[
    \begin{array}{l}
    -0.2970
    \\[1em]
    +0.9289
    \\[1em]
    -0.7804
    \end{array}
    \right]
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, once again, we get the same results that we found in the previous post using gradient descent. Here are the number of iterations required using Newton's method:

{% latex fig-27 %}
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
    \Sa(\mathbf{p})         & \text{4}
    \\[0.25em]\hline
    \Sb(\mathbf{p})         & \text{4}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Again, the number of iterations required to reach a solution using Newton's method is significantly less than the number of iterations required using gradient descent.

## Performance Considerations

We have demonstrated here that Newton's method can find a solution to the coin toss problem in far fewer iterations than the gradient descent method used in the previous post. Newton's method requires only a handful of iterations versus the tens of thousands or even hundreds of thousands of iterations required using gradient descent. But that doesn't necessarily mean Newton's method is a more efficient approach.

While both methods must evaluate a gradient in each iteration, Newton's method must also compute the values of a Jacobian matrix that is squared the size of the gradient vector. Furthermore, each iteration of Newton's method must also compute the inverse of the Jacobian matrix, which might be a computationally expensive task. These two methods might have very different algorithmic complexity characteristics in a larger problem space. Imagine a model of the coin toss game with ten, twenty, or even fifty flips per round.

Some of the other optimization and root finding methods mentioned in the previous post might have better performance characteristics than either Newton's method or gradient descent. The best way to find out might be to try out each method on larger problem sizes and empirically measure how well they perform.

{% accompanying_src_link %}
