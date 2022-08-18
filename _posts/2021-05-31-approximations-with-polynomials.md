---
layout: post
title: Approximations with Polynomials
---

The [previous post]({% post_url 2021-03-24-approximating-the-target-distribution %}) demonstrates the use of biases derived from a simple line formula to find an approximate solution to the weighted coin toss problem. In this post, I want to expand on some of these ideas using various polynomial formulas to describe the weights of the biased coins. As this experiment demonstrates, higher order polynomials do seem to yield better results.

<!--excerpt-->

## Linear Polynomial

For the first example, let's start with a linear polynomial. This is similar to the two-parameter formula used in the previous post, but with a larger range of possible slopes. The polynomial formula looks like this:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    f(x) = ax + b
    \end{displaymath}
    \end{document}
{% endlatex %}

Our objective is to find the coefficients that give us the lowest [approximation error]({% post_url 2021-03-24-approximating-the-target-distribution %}#approximation-error), as described in the previous post. But instead of optimizing the coefficients directly, we can use the weights of the biased coins in the +1 and +2 states as a proxy. Here is the relationship:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = a\1(1) + b
    \\[1em]
    p_2 & = a\1(2) + b
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can rearrange these two equations to get the values of the coefficients in terms of the weights of these two biased coins:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    a & = \phantom{-}\mathllap{-}p_1 + p_2
    \\[1em]
    b & = \phantom{-}\mathllap{2}p_1 - p_2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using these two coefficients in the polynomial formula, we can then compute the weights of each one of the biased coins in the model:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    p_i = f(i), \quad \forall i \in \{\, 1, \dots, n - 1 \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

But there is one small problem. The computed value might be outside the range of possible values for the coin's bias. Remember, a coin cannot land on heads more than 100% of the time or less than 0% of the time. We can get around this by capping the value at the lower and upper bounds:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(x) =
    \begin{dcases}
    0 & \quad \text{if $x < 0$}
    \\
    1 & \quad \text{if $x > 1$}
    \\
    x & \quad \text{otherwise}
    \end{dcases}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This capping function can be composed with the polynomial function to give us a valid computed value for the weights of the biased coins:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    p_i = (g \circ f)(i), \quad \forall i \in \{\, 1, \dots, n - 1 \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

So now, suppose we start with an initial guess for the weights of the coins in the +1 and +2 states. From this initial guess, we can compute the weights for all coins based on the polynomial formula. We can then compute the expected outcome of the coin toss game given the set of weights. Once we know the expected outcome, we can compare that to the target distribution we are trying to approximate. In this example, we'll use the same exponential distribution that was used in the last two posts as our target distribution:

{% chart fig-07-target-pmfunc.svg %}

With this technique, we can now compute the approximation error for all possible combinations of inputs. Since there are two variables in the case of a linear polynomial, we can create a surface plot or a heatmap of the error function. Here is a surface plot of the approximation error across the range of possible values:

{% chart fig-08-estimate-0-surface.svg %}

To find the point with the smallest approximation error, we can start with an arbitrary initial guess and use a hill climbing algorithm to find the most optimal approximation, just as we did in the previous post. But this time around, let's use a hill climbing algorithm that can move diagonally in addition to vertically and horizontally. Here are the paths taken by the hill climbing algorithm from several different starting points:

{% chart fig-09-estimate-1-heatmap.svg %}
{% chart fig-10-estimate-2-heatmap.svg %}
{% chart fig-11-estimate-3-heatmap.svg %}
{% chart fig-12-estimate-4-heatmap.svg %}

Notice that the last one doesn't converge to the same point as the other three. Instead of converging to the global minimum, it arrives at a point that seems to be located on either a local minimum, a flat plateau, or perhaps a narrow ridge. To avoid this kind of hazard, we need to be careful about choosing the starting point when using this optimization technique. The best bet is probably to choose a starting point as close to the optimum as possible. Here is the optimal result for the linear polynomial example:

{% chart fig-13-estimate-0-biases-fitted.svg %}
{% chart fig-14-estimate-0-pmfunc-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00001164
    \end{displaymath}
    \end{document}
{% endlatex %}

Compare this result to the [approximation for the same target distribution]({% post_url 2021-03-24-approximating-the-target-distribution %}#approximating-an-exponential-distribution) found in the previous post. In this case, the results are identical. But keep in mind these two results might not have necessarily been the same had we used a different target distribution. Let's see what happens if we use this approximation technique with higher order polynomials.

## Quadratic Polynomial

If we use a quadratic polynomial instead of a linear polynomial, can we find a better approximation? Let's find out. Here is the formula for a quadratic polynomial:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    f(x) = ax^2 + bx + c
    \end{displaymath}
    \end{document}
{% endlatex %}

Using the same technique as before, we'll use the biases of the coins in the +1, +2, and +3 states as a proxy for the coefficients. Here is the relationship:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = a\1(1)^2 + b\1(1) + c
    \\[1em]
    p_2 & = a\1(2)^2 + b\1(2) + c
    \\[1em]
    p_2 & = a\1(3)^2 + b\1(3) + c
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

To rearrange these three equations to state the coefficients in terms of the three biased coins, the best approach might be to use Gauss--Jordan elimination. We can represent the equations above in augmented matrix form, like this:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{wr{0.75em}}
    \begin{matrix}{xxx@{\quad}|@{\quad}l}
    1 & 1 & 1 & p_1
    \\[1em]
    4 & 2 & 1 & p_2
    \\[1em]
    9 & 3 & 1 & p_3
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

From here, we can perform a series of elementary row operations to convert the matrix above into the reduced row echelon form:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    \newcolumntype{x}{wr{0.75em}}
    \begin{matrix}{xxx@{\quad}|@{\quad}l}
    1 & 0 & 0 & +0.5 p_1 - 1.0 p_2 + 0.5 p_3
    \\[1em]
    0 & 1 & 0 & -2.5 p_1 + 4.0 p_2 - 1.5 p_3
    \\[1em]
    0 & 0 & 1 & +3.0 p_1 - 3.0 p_2 + 1.0 p_3
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we do that, we can easily express the three coefficients of the quadratic polynomial in terms of the weights of the three biased coins:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    a & = +0.5 p_1 - 1.0 p_2 + 0.5 p_3
    \\[1em]
    b & = -2.5 p_1 + 4.0 p_2 - 1.5 p_3
    \\[1em]
    c & = +3.0 p_1 - 3.0 p_2 + 1.0 p_3
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

And now that we know how to compute our coefficients, we can use the same technique that was used in the previous section to find the optimal solution. Here it is:

{% chart fig-21-estimate-5-biases-fitted.svg %}
{% chart fig-22-estimate-5-pmfunc-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00001117
    \end{displaymath}
    \end{document}
{% endlatex %}

This result is very similar to the one found in the previous section. The fitted polynomial has only a slight curve to it. As measured by the cost function, it is only a marginal improvement. While it's a good approximation, there doesn't seem to be much benefit to using a quadratic polynomial over a linear polynomial. At least in this instance.

## Cubic Polynomial

If we use a cubic polynomial instead of a quadratic polynomial, will the results be significantly better? Again, let's find out. Here is the formula for a cubic polynomial:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    f(x) = ax^3 + bx^2 + cx + d
    \end{displaymath}
    \end{document}
{% endlatex %}

Using the same technique again, we can use the biases of the coins in the +1, +2, +3, and +4 states as a proxy for the four coefficients. Here is the relationship:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = a\1(1)^3 + b\1(1)^2 + c\1(1) + d
    \\[1em]
    p_2 & = a\1(2)^3 + b\1(2)^2 + c\1(2) + d
    \\[1em]
    p_3 & = a\1(3)^3 + b\1(3)^2 + c\1(3) + d
    \\[1em]
    p_4 & = a\1(4)^3 + b\1(4)^2 + c\1(4) + d
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can use Gauss--Jordan elimination as we did before to express the four coefficients in terms of the weights of the four biased coins. And like we did before, we can use the hill climbing optimization method to find the optimum result:

{% chart fig-26-estimate-6-biases-fitted.svg %}
{% chart fig-27-estimate-6-pmfunc-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex fig-28 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00000021
    \end{displaymath}
    \end{document}
{% endlatex %}

The fitted polynomial here exhibits a noticeable curve. This result is quite a bit better than the one found using a quadratic polynomial. The error is much smaller, indicating a much better approximation. Compare this approximation to the [exact results found by a different method]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}#example-with-exponential-distribution) in a previous study. This approximation is very similar.

## Successful Experiment

This has turned out to be a successful experiment. I want to explore this technique further. Specifically, I want to use this technique to find approximate solutions for larger models with a lot more than ten coin toss events per round. The code I've written for this study runs too slowly for larger models, but I think there is room for improvement in terms of runtime performance.

Also, I'm curious to know what degree of a polynomial would be required to use this technique to find an exact solution instead of just an approximation. At most, I think a polynomial of a degree equal to the number of coin toss events per round would be required. But I suspect a smaller order polynomial might be sufficient. It might also be interesting to experiment with this approximation technique using sinusoidal functions instead of polynomials.

{% accompanying_src_link %}
