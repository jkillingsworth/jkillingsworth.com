---
layout: post
title: Higher Order Polynomial Approximation
---

This is an extension of one of my earlier posts on [polynomial approximations]({% post_url 2021-05-31-approximations-with-polynomials %}). Previously, I demonstrated how to find approximate solutions to the weighted coin toss problem using first, second, and third order polynomials to describe the weights of the biased coins. In this post, I demonstrate a generalized method for applying this technique using higher order polynomials.

<!--excerpt-->

## Generalized Method

Assume we're using a model of the coin toss game with an arbitrary number of coin toss events per round. The purpose of this technique is to use a polynomial of an arbitrary degree to describe the weights of the biased coins. Given the degree of the polynomial, let's define the following:

{% latex 1 fig-01 %}
    \begin{document}
    \begin{displaymath}
    m = (\text{degree of polynomial}) + 1
    \end{displaymath}
    \end{document}
{% endlatex %}

This number tells us how many coefficients are in our polynomial. It also defines the size of a matrix we will use to compute the values of the coefficients. Let's assume this number is smaller than the number of coin toss events:

{% latex 1 fig-02 %}
    \begin{document}
    \begin{displaymath}
    m < n
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is what our polynomial function looks like in expanded form:

{% latex 1 fig-03 %}
    \begin{document}
    \begin{displaymath}
    f(x) = a_0 + a_1 x + a_2 x^2 + \dots + a_{(m - 1)} x^{(m - 1)}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is what it looks like using a more compact notation:

{% latex 1 fig-04 %}
    \begin{document}
    \begin{displaymath}
    f(x) = \sum_{i = 0}^{m - 1} a_i x^i
    \end{displaymath}
    \end{document}
{% endlatex %}

Recall from the last few posts the iterative procedure we used to find approximate solutions to the coin toss problem using polynomials to represent the weights of the biased coins. What we need to do now is find the values of the coefficients based on a subset of the weights of the biased coins. Let's use the following notation to represent this subset as a vector:

{% latex 1 fig-05 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{p}_k
    =
    \begin{matrix}{c}
    p_1
    \\[1em]
    p_2
    \\[1em]
    \vdots
    \\[1em]
    p_m
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

This subset of the weights represents a guess for a particular iteration of the optimization process. It is a set of arguments we can use to find the corresponding coefficients. And knowing the coefficients, we can then compute the remaining weights based on the polynomial. Let's use the following notation to represent the coefficients as a vector:

{% latex 1 fig-06 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{a}_k
    =
    \begin{matrix}{c}
    a_0
    \\[1em]
    a_1
    \\[1em]
    \vdots
    \\[1em]
    a_{(m - 1)}
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we know the values of the coefficients, we can compute the values for the remaining weights and then calculate the value of the cost function. We repeat this process at each iteration to find incrementally better solutions. But how do we compute the values of the coefficients? Consider the following matrix:

{% latex 1 fig-07 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{M}
    =
    \newcolumntype{x}{wc{1.5em}}
    \begin{matrix}{xxxc}
    1^0    & 1^1    & \cdots & 1^{(m - 1)}
    \\[1em]
    2^0    & 2^1    & \cdots & 2^{(m - 1)}
    \\[1em]
    \vdots & \vdots & \ddots & \vdots
    \\[1em]
    m^0    & m^1    & \cdots & m^{(m - 1)}
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

With this matrix, we can set up a system of equations. The matrix multiplied by the coefficient vector is equal to the argument vector representing a subset of the weights of the biased coins. Here is this relationship expressed as a matrix equation:

{% latex 1 fig-08 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{M} \mathbf{a}_k = \mathbf{p}_k
    \end{displaymath}
    \end{document}
{% endlatex %}

Thus, we have a system of equations. Using the inverse of the matrix, we can rearrange the matrix equation above to express the coefficients in terms of the inputs depicting a subset of the weights of the biased coins:

{% latex 1 fig-09 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{a}_k = \mathbf{M}^{-1} \mathbf{p}_k
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have computed the coefficients, we can use the polynomial to compute all of the weights of the biased coins in the same manner as described in the last few posts. Note that we only need to compute the matrix and its inverse one time. They do not need to be recomputed for each iteration of the optimization process. In the examples that follow, we will use the Nelder--Mead method instead of the hill climbing method as the iterative method to find the most optimal solution.

## Example with Second Order Polynomial

To illustrate how this technique works, let's do an example using a second order polynomial. This example is similar to [another example]({% post_url 2021-05-31-approximations-with-polynomials %}#quadratic-polynomial) illustrated in a previous post. We'll start with the following definition:

{% latex 1 fig-10 %}
    \begin{document}
    \begin{displaymath}
    m = 2 + 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is what our second order polynomial function looks like:

{% latex 1 fig-11 %}
    \begin{document}
    \begin{displaymath}
    f(x) = a_0 + a_1 x + a_2 x^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the matrix for a second order polynomial:

{% latex 1 fig-12 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{M}
    =
    \begin{matrix}{ccc}
    1 & 1 & 1
    \\[1em]
    1 & 2 & 4
    \\[1em]
    1 & 3 & 9
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the inverse of the matrix:

{% latex 1 fig-13 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{M}^{-1}
    =
    \begin{matrix}{ccc}
    +3.0 & -3.0 & +1.0
    \\[1em]
    -2.5 & +4.0 & -1.5
    \\[1em]
    +0.5 & -1.0 & +0.5
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

And here is how we express the coefficients in terms of the weights:

{% latex 1 fig-14 %}
    \begin{document}
    \begin{displaymath}
    \begin{matrix}{c}
    a_0
    \\[1em]
    a_1
    \\[1em]
    a_2
    \end{matrix}
    =
    \begin{matrix}{c}
    + 3.0 p_1 - 3.0 p_2 + 1.0 p_3
    \\[1em]
    - 2.5 p_1 + 4.0 p_2 - 1.5 p_3
    \\[1em]
    + 0.5 p_1 - 1.0 p_2 + 0.5 p_3
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

Consistent with examples from some of the previous posts, let's use the following probability mass function as the target distribution we want to find an approximation for:

{% chart fig-15-target-pmfunc-n-10.svg %}

Here is the solution found after 161 iterations:

{% chart fig-16-estimate-n-10-1-biases-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex 1 fig-17 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00001117
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, based on the value of the cost function at the most optimal point, this is not an exact solution. But it is still a pretty good approximation. And this solution matches the one found in the [other example]({% post_url 2021-05-31-approximations-with-polynomials %}#quadratic-polynomial).

## Example with Sixth Order Polynomial

To demonstrate the capabilities of this approximation technique, let's showcase another example. For this example, let's use a sixth order polynomial. Using the same approach as before, we'll start with the following definition:

{% latex 1 fig-18 %}
    \begin{document}
    \begin{displaymath}
    m = 6 + 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is what our sixth order polynomial function looks like:

{% latex 1 fig-19 %}
    \begin{document}
    \begin{displaymath}
    f(x) = a_0 + a_1 x + a_2 x^2 + a_3 x^3 + a_4 x^4 + a_5 x^5 + a_6 x^6
    \end{displaymath}
    \end{document}
{% endlatex %}

To be consistent with the example in the previous section, we'll use the same target distribution. Here is the solution found after 1,308 iterations:

{% chart fig-20-estimate-n-10-2-biases-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex 1 fig-21 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00000000
    \end{displaymath}
    \end{document}
{% endlatex %}

Based on the value of the cost function at the most optimal point, this appears to be an exact solution. Applying this technique with fourth and fifth order polynomials also yields seemingly exact solutions. And it does so with fewer and less computationally expensive iterations. It is not clear to me what minimum degree of a polynomial is required to find an exact solution in the general case when there is an arbitrary number of coin toss events.

{% accompanying_src_link %}
