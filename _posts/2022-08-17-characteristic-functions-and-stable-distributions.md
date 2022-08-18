---
layout: post
title: Characteristic Functions and Stable Distributions
---

In the [previous post]({% post_url 2022-07-07-generalized-normal-distributions %}), I explored the use of the generalized normal distribution to model the price movements of financial instruments. This approach offered better fitting distributions than the normal and Laplace distributions studied in earlier posts. But the shape of the fitted distributions still didn't quite match the shape of the histogram. In this post, I want to explore a class of probability distributions known as L&eacute;vy alpha-stable distributions. And to explore these distributions, we need to understand characteristic functions.

<!--excerpt-->

## Characteristic Functions

In my mind, the most intuitive representation of a probability distribution is a probability density function. But there are other possible ways to represent a probability distribution. You could, for example, use a cumulative distribution function instead. Another alternative is to use a characteristic function. The characteristic function of a probability distribution is just the Fourier transform of its density function:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \varphi(t)
    =
    \int_{-\infty}^{\infty} f(x) \, e^{i x t} \, \dderiv x
    \end{displaymath}
    \end{document}
{% endlatex %}

As shown above, if you know the formula for the probability density function, you can compute the characteristic function by evaluating the integral. Likewise, if you know the formula for the characteristic function, you can work out the probability density function by taking the inverse transform, as shown below:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    f(x)
    =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty} \varphi(t) \, e^{-i x t} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

There are a variety of different purposes for which characteristic functions can be useful. Characteristic functions can be used to simplify the convolution operation, for example. There are also probability distributions that can be cleanly represented using characteristic functions but do not have probability density functions that can be expressed in closed form.

## Stable Distributions

Except for a small number of cases, the L&eacute;vy alpha-stable distribution does not have a probability density function that can be expressed analytically. However, it is possible to represent the probability density function in terms of the characteristic function. Here is the general form of the characteristic function for stable distributions:

{% latex fig-03 %}
    \newcommand{\sgn}{\operatorname{sgn}}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    &
    \varphi(t \mid \mu, \gamma, \alpha, \beta)
    =
    \exp\2\brace2[]
    {
    i \mu t - |\gamma t|^\alpha \brace1(){ 1 - i \beta \sgn\2(t) \2 \varPhi }
    }
    \\[1em]
    &
    \varPhi =
    \begin{dcases}
    \tan\2\brace1(){ \tfrac{\pi \alpha}{2} } & \quad \text{if $\alpha \neq 1$}
    \\[0.5em]
    -\tfrac{2}{\pi} \log |t|                 & \quad \text{if $\alpha = 1$}
    \end{dcases}
    \\[1em]
    &
    \begin{aligned}
    & \mu    && \text{is the location parameter}
    \\
    & \gamma && \text{is the scale parameter}
    \\
    & \alpha && \text{is the shape parameter}
    \\
    & \beta  && \text{is the skew parameter}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The location parameter can be negative or positive. The scale parameter is always a positive number. The shape parameter must be a positive number no greater than two, while the skew parameter can range from negative one to positive one. For the sake of simplicity, let's drop the skew parameter and assume the skew is always zero. Omitting the skew parameter simplifies the characteristic function to the following:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \varphi(t \mid \mu, \gamma, \alpha)
    =
    \exp\2\brace1(){ i \mu t - |\gamma t|^\alpha }
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the characteristic function for the symmetric alpha-stable distribution. The corresponding probability density function is symmetric with respect to the location parameter. Using the characteristic function above, we can compute the probability density function like this:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \gamma, \alpha)
    =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    e^{i \mu t - |\gamma t|^\alpha} e^{-i x t} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

We can use numerical integration to evaluate this function and plot charts for specific parameter values. Holding the location and scale parameters constant and varying the shape parameter, we can see what the shape of the density function looks like for different values of the shape parameter. Here are some illustrations:

{% chart fig-06-shape-0.45.svg %}
{% chart fig-07-shape-0.55.svg %}
{% chart fig-08-shape-0.75.svg %}
{% chart fig-09-shape-1.00.svg %}
{% chart fig-10-shape-1.35.svg %}
{% chart fig-11-shape-2.00.svg %}

There are two special cases in the examples illustrated above. When the shape parameter is equal to two, the stable distribution is equivalent to a normal distribution. When the shape parameter is equal to one, the stable distribution is equivalent to a Cauchy distribution. For these two special cases, the probability density function can be expressed in closed form.

## Normal Distribution

The normal distribution is a special case of a stable distribution when the shape parameter equals two. Plug the following values into the characteristic function:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    \alpha = 2, \quad \gamma = \frac{\sigma}{\sqrt 2}
    \end{displaymath}
    \end{document}
{% endlatex %}

With these values, the characteristic function now looks like this:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    \varphi(t \mid \mu, \sigma)
    =
    \exp\2\brace1(){ i \mu t - \tfrac{1}{2} \sigma^2 t^2 }
    \end{displaymath}
    \end{document}
{% endlatex %}

And we can now find the probability density function based on the characteristic function:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \sigma)
    =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    e^{i \mu t - \tfrac{1}{2} \sigma^2 t^2} e^{-i x t} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

Evaluating the integral, we can arrive at the following expression of the normal distribution:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp\2\brace4[]{ -\frac{(x - \mu)^2}{2 \sigma^2} }
    \end{displaymath}
    \end{document}
{% endlatex %}

As shown above, by holding the shape parameter to a fixed value of two, the density function of the stable distribution is equivalent to that of a normal distribution.

## Cauchy Distribution

The Cauchy distribution is a special case of a stable distribution when the shape parameter equals one. Plug the following values into the characteristic function:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    \alpha = 1, \quad \gamma = \lambda
    \end{displaymath}
    \end{document}
{% endlatex %}

With these values, the characteristic function now looks like this:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \varphi(t \mid \mu, \lambda)
    =
    \exp\2\brace1(){ i \mu t - |\lambda t| }
    \end{displaymath}
    \end{document}
{% endlatex %}

And we can now find the probability density function based on the characteristic function:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \lambda)
    =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    e^{i \mu t - |\lambda t|} e^{-i x t} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

Evaluating the integral, we can arrive at the following expression of the Cauchy distribution:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \sigma)
    =
    \frac{1}{\pi} \cdot \frac{\lambda}{(x - \mu)^2 + \lambda^2 }
    \end{displaymath}
    \end{document}
{% endlatex %}

As shown above, by holding the shape parameter to a fixed value of one, the density function of the stable distribution is equivalent to that of a Cauchy distribution.

## Numerical Parameter Estimation

We can use the maximum likelihood method in the same fashion we did in the previous post to fit the location, scale, and shape parameters to a stable distribution. Here is the likelihood function that we want to maximize:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    L(\mu, \gamma, \alpha \mid \mathbf{x})
    =
    \prod_{i = 1}^{n} f(x_i \mid \mu, \gamma, \alpha)
    \end{displaymath}
    \end{document}
{% endlatex %}

To fit a stable distribution to an observed set of data, we need to find the parameter values that maximize this function. For the examples appearing later in this post, we're going to use numerical optimization to find the most optimal parameter values. The cost function we're going to use is the negation of the logarithm of the likelihood function:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    C(\mu, \gamma, \alpha)
    =
    -\sum_{i = 1}^{n} \log\2\brace2[]{ f(x_i \mid \mu, \gamma, \alpha) }
    \end{displaymath}
    \end{document}
{% endlatex %}

Using numerical optimization methods, we need to start with an initial guess. As with the previous post, I've chosen to start with a shape parameter value of two:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    \alpha_0 = 2
    \end{displaymath}
    \end{document}
{% endlatex %}

This implies a normal distribution. The location and scale parameters for a normal distribution are easy to compute:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    \mu_0 = \frac{1}{n} \sum_{i = 1}^{n} x_i
    , \quad
    \gamma_0 = \sqrt{\displaystyle \frac{1}{2n} \sum_{i = 1}^{n} \2 (x_i - \mu_0)^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

These are the initial parameter values for the numerical optimization method. Like before, I'm using a third-party implementation of the Nelder--Mead algorithm. This is an unconstrained method, so it's possible for the optimization process to stray outside the range of valid parameter values. To offset this, I've modified the cost function to return a penalty value if the optimization procedure wanders outside the valid range of values for the shape parameter.

## Numerical Integration

In addition to using numerical optimization to find the optimum parameter values, we also need to use numerical integration to evaluate the probability density function for stable distributions. I'm using an implementation of the Gauss--Kronrod method for the examples in this post. Since numerical integration is a compute-intensive evaluation method, it can be beneficial to consider the runtime performance of the evaluation of the integrand. Consider the following:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f(x \mid \mu, \gamma, \alpha)
    & =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    e^{i \mu t - |\gamma t|^\alpha} e^{-i x t} \, \dderiv t
    \\[1em]
    & =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    e^{i (\mu t - x t)} e^{-|\gamma t|^\alpha} \, \dderiv t
    \\[1em]
    & =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    \brace2[]
    {
    \cos\2\brace1(){ \mu t - x t }
    + i
    \sin\2\brace1(){ \mu t - x t }
    }
    \, e^{-|\gamma t|^\alpha} \, \dderiv t
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The integrand has a complex exponential, so we can use Euler's formula to separate the real part from the imaginary part. And if we can assume the final result is always a real number, we can simply drop the imaginary part entirely:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \gamma, \alpha)
    =
    \frac{1}{2 \pi} \int_{-\infty}^{\infty}
    \cos\2\brace1(){ \mu t - x t } \, e^{-|\gamma t|^\alpha} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

Notice that, in the formula above, the integrand is symmetric with respect to zero. That means we can do a little trick where we just integrate over the positive half of the interval and then double the result:

{% latex fig-26 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \gamma, \alpha)
    =
    \frac{1}{\pi} \int_{0}^{\infty}
    \cos\2\brace1(){ \mu t - x t } \, e^{-|\gamma t|^\alpha} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

With the density function for the stable distribution in this form, the runtime performance seems to be improved by a factor of five. There are probably other optimizations that can be made as well, but this is fast enough for the examples in this post.

## Microsoft Stock Prices

Now let's look at some examples of fitting stable distributions to real-life data. The first example is based on the historical stock prices of Microsoft Corporation. Like we did in the last post, we'll take the logarithm of the daily closing prices, compute the first differences, and then put the data in a histogram. The following charts show the histogram overlaid with the fitted density functions for the normal, Cauchy, and generalized symmetric alpha-stable distributions, respectively:

{% chart fig-27-fitted-MSFT-N.svg %}
{% chart fig-28-fitted-MSFT-C.svg %}
{% chart fig-29-fitted-MSFT-S.svg %}

The fitted symmetric alpha-stable distribution has the following shape parameter:

{% latex fig-30 %}
    \begin{document}
    \begin{displaymath}
    \alpha = 1.6462
    \end{displaymath}
    \end{document}
{% endlatex %}

As expected, the density function for the generalized form of the stable distribution appears to be a better fit than that of either the normal distribution or the Cauchy distribution. Worth noting, however, is that the fitted curve doesn't seem to fit the histogram as tightly as the generalized normal density function studied in the previous post---subjectively speaking, anyways.

## Bitcoin Prices

This example uses historical bitcoin prices. We'll do the same thing we did in the previous example, taking the first differences of the logarithm of the daily closing prices and putting the data in a histogram. The following charts show the histogram overlaid with the fitted density functions for the normal, Cauchy, and generalized symmetric alpha-stable distributions, respectively:

{% chart fig-31-fitted-BTCUSD-N.svg %}
{% chart fig-32-fitted-BTCUSD-C.svg %}
{% chart fig-33-fitted-BTCUSD-S.svg %}

The fitted symmetric alpha-stable distribution has the following shape parameter:

{% latex fig-34 %}
    \begin{document}
    \begin{displaymath}
    \alpha = 1.3206
    \end{displaymath}
    \end{document}
{% endlatex %}

In this example, the fitted density function for the Cauchy distribution looks like a better fit than the density function for the generalized form of the stable distribution. This is not what I was expecting. The Cauchy distribution is a special case of the symmetric stable distribution, so the stable distribution should fit at least as well as the Cauchy distribution. But that doesn't seem to be the case here. I am bothered by this result.

## Natural Gas Prices

The third example uses the daily closing prices of a natural gas ETF. Once again, we'll take the logarithm of the price quotes, compute the first differences, and then put the data into a histogram. The following charts show the histogram overlaid with the fitted density functions for the normal, Cauchy, and generalized symmetric alpha-stable distributions, respectively:

{% chart fig-35-fitted-UNG-N.svg %}
{% chart fig-36-fitted-UNG-C.svg %}
{% chart fig-37-fitted-UNG-S.svg %}

The fitted symmetric alpha-stable distribution has the following shape parameter:

{% latex fig-38 %}
    \begin{document}
    \begin{displaymath}
    \alpha = 1.7999
    \end{displaymath}
    \end{document}
{% endlatex %}

In this case, the density function for the generalized stable distribution seems to fit quite nicely. It looks like a better fit than the density function for normal distribution or the Cauchy distribution. And the shape of the density function looks like a better match than that of the generalized normal distribution used in the last post.

## Better Methods

I am not happy with the results of this experiment. Compared to the generalized normal distribution used in the last post, the shape of the density function for the L&eacute;vy alpha-stable distribution seems to be a better fit for the kind of data used in the examples above. However, fitting the parameters using the maximum likelihood method doesn't always seem to give the best results. I am particularly disappointed with the results based on the bitcoin price data. And I think there might be better fitting parameters for the other data sets as well. Since stable distributions have heavy tails, I suspect that a dearth of sample data deep in the tails might be throwing off the parameter estimates. It might be worth doing a follow-up study to experiment with different fitting methods that might yield a better result.

{% accompanying_src_link %}
