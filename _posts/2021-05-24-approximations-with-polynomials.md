---
layout: post
title: Approximations with Polynomials
---

The [previous post]({% post_url 2021-03-24-approximating-the-target-distribution %}) demonstrated the use of biases derived from a simple line formula to find an approximate solution to the weighted coin toss problem. In this post, I want to expand on some of these ideas using various polynomial formulas to describe the weights of the biased coins. As this experiment demonstrates, higher order polynomials do yield better results as measured by the cost function. However, the optimal solutions approximated using these higher order polynomials are quite different from those found by other methods.

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
    p_1 & = a(1) + b
    \\[1em]
    p_2 & = a(2) + b
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can rearrange these two equations to get the values of the coefficients in terms of the weights of these two biased coins:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    a & = \phantom{-}\mathllap{-} p_1 + p_2
    \\[1em]
    b & = \phantom{-}\mathllap{2} p_1 - p_2
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
    0 & \quad \text{if } x < 0
    \\
    1 & \quad \text{if } x > 1
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
    p_i = (g \circ f)(i)
    , \quad \forall i \in \{\, 1, \dots, n - 1 \,\}
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

Notice that the last one doesn't converge to the same point as the other three. It converges to a local minimum instead of the global minimum. Unfortunately, when using the capping function described above, the cost function is not convex. This means we need to be careful about choosing the starting point when using this optimization technique. One way to get around this pitfall is to compute the approximation error at multiple points located at evenly spaced intervals, choose the best one, and then use that as the starting point for the hill climbing algorithm. Here is the optimal result for the linear polynomial example:

{% chart fig-13-estimate-5-biases-fitted.svg %}
{% chart fig-14-estimate-5-pmfunc-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00307193
    \end{displaymath}
    \end{document}
{% endlatex %}

As measured by the cost function, this approximation is slightly better than the [approximation for the same target distribution]({% post_url 2021-03-24-approximating-the-target-distribution %}#approximating-an-exponential-distribution) found in the previous post. Subjectively speaking, however, the biases don't seem to be a better approximation for the [exact results]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}#example-with-exponential-distribution) found earlier. Let's see what happens if we use higher order polynomials.

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
    p_1 & = a(1)^2 + b(1) + c
    \\[1em]
    p_2 & = a(2)^2 + b(2) + c
    \\[1em]
    p_2 & = a(3)^2 + b(3) + c
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

To rearrange these three equations to state the coefficients in terms of the three biased coins, the best approach might be to use Gauss--Jordan elimination. We can represent the equations above in augmented matrix form, like this:

{% latex fig-18 %}
    \usepackage{array}
    \begin{document}
    \begin{displaymath}
    \left[
    \newcolumntype{x}{>{$}wr{0.75em}<{$}}
    \begin{array}{xxx@{\quad}|@{\quad}l}
    1 & 1 & 1 & p_1
    \\[1em]
    4 & 2 & 1 & p_2
    \\[1em]
    9 & 3 & 1 & p_3
    \end{array}
    \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

From here, we can perform a series of elementary row operations to convert the matrix above into the reduced row echelon form:

{% latex fig-19 %}
    \usepackage{array}
    \begin{document}
    \begin{displaymath}
    \left[
    \newcolumntype{x}{>{$}wr{0.75em}<{$}}
    \begin{array}{xxx@{\quad}|@{\quad}l}
    1 & 0 & 0 & + 0.5 p_1 - 1.0 p_2 + 0.5 p_3
    \\[1em]
    0 & 1 & 0 & - 2.5 p_1 + 4.0 p_2 - 1.5 p_3
    \\[1em]
    0 & 0 & 1 & + 3.0 p_1 - 3.0 p_2 + 1.0 p_3
    \end{array}
    \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we do that, we can easily express the three coefficients of the quadratic polynomial in terms of the weights of the three biased coins:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    a & = + 0.5 p_1 - 1.0 p_2 + 0.5 p_3
    \\[1em]
    b & = - 2.5 p_1 + 4.0 p_2 - 1.5 p_3
    \\[1em]
    c & = + 3.0 p_1 - 3.0 p_2 + 1.0 p_3
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

And now that we know how to compute our coefficients, we can use the same technique that was used in the previous section to find the optimal solution. Here it is:

{% chart fig-21-estimate-6-biases-fitted.svg %}
{% chart fig-22-estimate-6-pmfunc-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00087928
    \end{displaymath}
    \end{document}
{% endlatex %}

That is not what I was expecting. It is a better solution as measured by the cost function, but it certainly doesn't look like it. The approximated probability mass function doesn't really have the same general shape as the target distribution, and the biases are completely different than those found using the linear polynomial.

## Cubic Polynomial

If we use a cubic polynomial instead of a quadratic polynomial, will the results be better or worse? Again, let's find out. Here is the formula for a cubic polynomial:

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
    p_1 & = a(1)^3 + b(1)^2 + c(1) + d
    \\[1em]
    p_2 & = a(2)^3 + b(2)^2 + c(2) + d
    \\[1em]
    p_3 & = a(3)^3 + b(3)^2 + c(3) + d
    \\[1em]
    p_4 & = a(4)^3 + b(4)^2 + c(4) + d
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can use Gauss-Jordan elimination like we did before to express the four coefficients in terms of the weights of the four biased coins. And like we did before, we can use the hill climbing optimization method to find the optimum result:

{% chart fig-26-estimate-7-biases-fitted.svg %}
{% chart fig-27-estimate-7-pmfunc-fitted.svg %}

Here is the value of the cost function at the most optimal point:

{% latex fig-28 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00000333
    \end{displaymath}
    \end{document}
{% endlatex %}

This result is even better still, at least as measured by the cost function. And the probability mass function of the expected outcome looks almost exactly like the target distribution we're trying to approximate. However, the biases themselves are still wildly different than any of the other results found.

## Failed Experiment

I consider this to be somewhat of a failed experiment. I was hoping that an approximation technique using polynomials to describe the weights of the biased coins would yield results very similar to those found in [previous studies]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}#example-with-exponential-distribution). My expectation was that higher order polynomials would yield similar results with increasing precision. As this experiment shows, however, this is not the case.

{% accompanying_src_link %}
