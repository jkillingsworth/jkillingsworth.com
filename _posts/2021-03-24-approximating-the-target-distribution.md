---
layout: post
title: Approximating the Target Distribution
---

In previous studies of the weighted coin toss game, our focus was on finding a set of weights for the biased coins that would yield a given target distribution for the expected outcome. In this post, I want to explore a different approach. Instead of finding an exact solution, I want to try finding an approximate solution using a set of weights based on a parameterized formula. This might produce an approximate solution that is good enough for practical purposes while also being easier to compute for a model with a large number of coin toss events per round.

<!--excerpt-->

## Biases Based on a Formula

The probability mass function of the possible outcomes of the coin toss game is a function of the biases of the weighted coins. Using the generalized Markov model presented in the [previous post]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}), the probability mass function can be computed for a game with any number of coin toss events per round and any combination of weights. Consider a coin toss game with ten flips per round. And suppose the coin being tossed is always a fair coin:

{% chart fig-01-evaluate-1-biases.svg %}
{% chart fig-02-evaluate-1-pmfunc.svg %}

In this case, we wind up with a binomial distribution for the distribution of expected outcomes. You can think of the weighted coin toss game as a system that maps a set of weights to a discrete probability distribution. The shape of the probability mass function is determined by the weights of the biased coins. Now suppose the weights of the biased coins are determined by a formula with a single parameter:

{% latex 1 fig-03 %}
    \begin{document}
    \begin{displaymath}
    p_i = 0.5 + i\1\brace3(){ \frac{a - 0.5}{n - 1} }
    ,
    \quad \forall i \in \{\, 0, \dots, n - 1 \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is an example of weights defined by the formula above and the probability distribution of the possible outcomes:

{% chart fig-04-evaluate-2-biases.svg %}
{% chart fig-05-evaluate-2-pmfunc.svg %}

Here is another example with a different parameter value:

{% chart fig-06-evaluate-3-biases.svg %}
{% chart fig-07-evaluate-3-pmfunc.svg %}

As you can see in the examples above, a single parameter can determine the shape of the probability mass function. The shape of the distribution appears to be either a stretched or compressed form of the binomial distribution, depending on whether the parameter is larger or smaller than 50%. Now let's consider another formula, this time with two parameters:

{% latex 1 fig-08 %}
    \begin{document}
    \begin{displaymath}
    p_i = a + (i - 1) \brace3(){ \frac{b - a}{n - 2} }
    ,
    \quad \forall i \in \{\, 1, \dots, n - 1 \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

Remember that, according to our model of the coin toss game, the coin in the initial state is always a fair coin:

{% latex 1 fig-09 %}
    \begin{document}
    \begin{displaymath}
    p_0 = 0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is an example of weights defined by the formula above and the probability distribution of the possible outcomes:

{% chart fig-10-evaluate-4-biases.svg %}
{% chart fig-11-evaluate-4-pmfunc.svg %}

Here is another example with different parameter values:

{% chart fig-12-evaluate-5-biases.svg %}
{% chart fig-13-evaluate-5-pmfunc.svg %}

Again, the shape of the probability distribution can be determined by the parameters. Using two parameters gives a greater degree of variability than using a single parameter. However, a formula with only a few parameters cannot represent all possible shapes that the probability mass function can have. If we are trying to find a set of parameters that fit a given target distribution, we might not be able to get an exact match.

## Approximation Error

We want to use the two-parameter formula described in the previous section to find an approximate solution for a given target distribution. For some distributions, we might be able to find an exact solution. For others, we want to find the closest match. To do this, we need to be able to quantify the difference between the target distribution and a proposed approximation. The number of unique data points we need to compare is given by:

{% latex 1 fig-14 %}
    \begin{document}
    \begin{displaymath}
    m =
    \begin{dcases}
    \frac{n + 2}{2} & \quad \text{if $n$ is even}
    \\[0.5em]
    \frac{n + 1}{2} & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is fewer than the total number of possible outcomes. In our model, the distribution of possible outcomes is always symmetrical about the initial state, so we only bother looking at values on one side. For each element, we can compute an error term that is the difference between the target value and the approximate value. We can then take the sum of the square of the errors to give us a value that quantifies the approximation error:

{% latex 1 fig-15 %}
    \begin{document}
    \begin{displaymath}
    C = \sum_{i = 1}^{m} \2 \brace1(){ r_{k,\sscr{target}} - r_{k,\sscr{approx}} }^2, \quad
    k =
    \begin{dcases}
    2i - 2 & \quad \text{if $n$ is even}
    \\
    2i - 1 & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

This allows us to quantify the approximation error and determine whether one approximation is better than another. A lower value would mean a better approximation. An exact match would be zero. In the following sections, we'll use this as a cost function for which we can find the optimal set of parameters using a hill climbing algorithm.

## Approximating a Binomial Distribution

In this first example, let's find a suitable approximation of the binomial distribution using the two-parameter formula for the weights. You might be able to guess what the solution is for this example, but let's go through the motions. Here is our target distribution:

{% chart fig-16-estimate-1-pmfunc-target.svg %}

Given the target distribution, we can compute the approximation error for all possible approximations using the two-parameter formula for the weights of the biased coins. Here is a surface plot of the error function:

{% chart fig-17-estimate-1-surface.svg %}

We want to find the lowest point on the surface. To do this, we can use a simple hill climbing algorithm like the ones used in previous posts. Here is a trace of the paths taken by the hill climbing algorithm for three different starting points:

{% chart fig-18-estimate-1-trace-1.svg %}
{% chart fig-19-estimate-1-trace-2.svg %}
{% chart fig-20-estimate-1-trace-3.svg %}

Since the error function is convex, the algorithm always converges to the best solution, regardless of the initial guess. Here is the optimal set weights and corresponding probability mass function for the approximate solution found:

{% chart fig-21-estimate-1-biases-fitted.svg %}
{% chart fig-22-estimate-1-pmfunc-fitted.svg %}

Once we have found the optimal solution, we can then compute the sum of squared errors to give us a measure of how well the optimal solution approximates the target distribution:

{% latex 1 fig-23 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00000000
    \end{displaymath}
    \end{document}
{% endlatex %}

In this case, it's an exact match. The error function is zero at the optimal point. The approximated distribution is identical to the target distribution. As you might have guessed, a set of weights in which all coins are fair is the solution that yields the target distribution.

## Approximating a Triangular Distribution

Let's take a look at another example. What happens if we try to approximate a triangular distribution? The solution for this example might not be as obvious as it was for the last one. Here is what the target distribution looks like:

{% chart fig-24-estimate-2-pmfunc-target.svg %}

Given the target distribution, we can compute the approximation error for all possible approximations using the two-parameter formula for the weights of the biased coins. Here is a surface plot of the error function:

{% chart fig-25-estimate-2-surface.svg %}

We want to find the lowest point on the surface. To do this, we can use a simple hill climbing algorithm like the ones used in previous posts. Here is a trace of the paths taken by the hill climbing algorithm for three different starting points:

{% chart fig-26-estimate-2-trace-1.svg %}
{% chart fig-27-estimate-2-trace-2.svg %}
{% chart fig-28-estimate-2-trace-3.svg %}

Since the error function is convex, the algorithm always converges to the best solution, regardless of the initial guess. Here is the optimal set weights and corresponding probability mass function for the approximate solution found:

{% chart fig-29-estimate-2-biases-fitted.svg %}
{% chart fig-30-estimate-2-pmfunc-fitted.svg %}

Once we have found the optimal solution, we can then compute the sum of squared errors to give us a measure of how well the optimal solution approximates the target distribution:

{% latex 1 fig-31 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00000405
    \end{displaymath}
    \end{document}
{% endlatex %}

The error is not zero, so we don't have an exact match. But eyeballing the approximated distribution, it looks like a pretty close estimate of the target distribution. The weights are similar to those found for the triangular distribution in the [previous post]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}#example-with-triangular-distribution).

## Approximating an Exponential Distribution

Now let's consider a third example. In this example, we want to try to approximate an exponential distribution. How close of a match do you think we can find using this estimation technique? Here is the target distribution we want to approximate:

{% chart fig-32-estimate-3-pmfunc-target.svg %}

Given the target distribution, we can compute the approximation error for all possible approximations using the two-parameter formula for the weights of the biased coins. Here is a surface plot of the error function:

{% chart fig-33-estimate-3-surface.svg %}

We want to find the lowest point on the surface. To do this, we can use a simple hill climbing algorithm like the ones used in previous posts. Here is a trace of the paths taken by the hill climbing algorithm for three different starting points:

{% chart fig-34-estimate-3-trace-1.svg %}
{% chart fig-35-estimate-3-trace-2.svg %}
{% chart fig-36-estimate-3-trace-3.svg %}

Since the error function is convex, the algorithm always converges to the best solution, regardless of the initial guess. Here is the optimal set weights and corresponding probability mass function for the approximate solution found:

{% chart fig-37-estimate-3-biases-fitted.svg %}
{% chart fig-38-estimate-3-pmfunc-fitted.svg %}

Once we have found the optimal solution, we can then compute the sum of squared errors to give us a measure of how well the optimal solution approximates the target distribution:

{% latex 1 fig-39 %}
    \begin{document}
    \begin{displaymath}
    C = 0.00001164
    \end{displaymath}
    \end{document}
{% endlatex %}

Again, the approximation error is a nonzero value, so we don't have an exact match. Nonetheless, the error is still quite small, and the shape of the approximated distribution still looks quite similar to that of the target distribution. Like the previous example, the weights are quite similar to those found for the exponential distribution in the [previous post]({% post_url 2021-02-25-generalizing-the-coin-toss-markov-model %}#example-with-exponential-distribution).

## Possible Improvements

In some cases, the minimum value of the error function might lie at a point outside the range of possible values for the weights. Since all of the weights of the biased coins to the right of the initial state must fall somewhere on or between the two parameter values, and since the parameters are limited to values between zero and one, we might not be able to find the best approximation using this approach. A more accurate approximation might be found by allowing the parameters to float outside the boundary. Taking this approach, we would have to cap the weights of the biased coins that would otherwise fall outside the range of valid values before computing the approximation error.

Another thing that might improve the accuracy of the approximation would be to use some sort of curve instead of a straight line for the formula that determines the weights of the biased coins. I think this would require more than two parameters.

{% accompanying_src_link %}
