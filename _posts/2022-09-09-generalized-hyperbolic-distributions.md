---
layout: post
title: Generalized Hyperbolic Distributions
---

The [generalized hyperbolic distribution](https://en.wikipedia.org/wiki/Generalised_hyperbolic_distribution) was pioneered by Ole Barndorff-Nielsen with applications related to wind-blown sand. There are several probability distributions that can be expressed as special cases of the generalized hyperbolic distribution, which is indicative of its versatility. As we shall see in this post, this distribution seems to work pretty well for modeling price fluctuations in financial markets.

<!--excerpt-->

## Probability Density Function

The generalized hyperbolic distribution is a continuous probability distribution with five parameters: a location parameter, a scale parameter, a shape parameter, a skew parameter, and a subclass parameter. The probability density function takes the following form:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    &
    f(x \mid \mu, \delta, \alpha, \beta, \lambda)
    =
    \sqrt{\frac{\alpha}{2 \pi}}
    \cdot
    \frac
    {
    \brace1(){ \alpha^2 - \beta^2 }^{\nicefrac{\lambda}{2}}
    }
    {
    \alpha^{\lambda}
    }
    \cdot
    \frac
    {
    K_{\lambda - \nicefrac{1}{2}}\brace1(){ \alpha \theta }
    \theta^{\lambda - \nicefrac{1}{2}}
    e^{\beta (x - \mu)}
    }
    {
    \delta^{\lambda}
    K_{\lambda}\brace1(){ \delta \sqrt{\alpha^2 - \beta^2} }
    }
    \\[1em]
    &
    \theta = \sqrt{\delta^2 + (x - \mu)^2}
    \\[1em]
    &
    \begin{aligned}
    & \mu     && \text{is the location parameter}
    \\
    & \delta  && \text{is the scale parameter}
    \\
    & \alpha  && \text{is the shape parameter}
    \\
    & \beta   && \text{is the skew parameter}
    \\
    & \lambda && \text{is the subclass parameter}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The location parameter and the subclass parameter can be negative or positive. The scale parameter and the shape parameter are always positive numbers. The skew parameter can be negative or positive, provided that its magnitude does not exceed that of the shape parameter. Note the use of the [Bessel function](https://en.wikipedia.org/wiki/Bessel_function) above. This formula uses the modified Bessel function of the second kind. One representation of this Bessel function looks like this:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    K_v(z)
    =
    \int_{0}^{\infty} e^{-z \cosh\2(t)} \cosh\2(v t) \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

There are several other ways to express this Bessel function as well. For the examples in this post, I am using a third-party implementation that returns a numerical approximation of this function. Be aware that this Bessel function may be called different names by different sources. If we drop the skew parameter and assume the probability distribution is always symmetrical, we can express the density function like this:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \delta, \alpha, \lambda)
    =
    \sqrt{\frac{\alpha}{2 \pi}}
    \cdot
    \frac
    {
    K_{\lambda - \nicefrac{1}{2}}\brace1(){ \alpha \sqrt{\delta^2 + (x - \mu)^2} }
    \brace1(){ \sqrt{\delta^2 + (x - \mu)^2} }^{\lambda - \nicefrac{1}{2}}
    }
    {
    \delta^{\lambda}
    K_{\lambda}\brace1(){ \delta \alpha }
    }
    \end{displaymath}
    \end{document}
{% endlatex %}

For the remainder of this post, let's use this density function and assume the skew is always zero. If we vary the scale parameter while holding the remaining parameters constant, we can see how the shape of the distribution is affected in the illustrations below:

{% chart fig-04-model-scale-1.00.svg %}
{% chart fig-05-model-scale-0.33.svg %}
{% chart fig-06-model-scale-0.01.svg %}

As you can see in the illustrations above, the center of the density function becomes more pointed as the scale parameter approaches zero. It looks almost like a Laplace distribution. Let's see what happens when we vary the shape parameter:

{% chart fig-07-model-shape-1.00.svg %}
{% chart fig-08-model-shape-0.33.svg %}
{% chart fig-09-model-shape-0.01.svg %}

As the shape parameter approaches zero, the tails tend to get fatter. Unlike the probability distributions examined in the previous posts, the scale parameter can influence the shape of the distribution in addition to the shape parameter. Varying the subclass parameter can also affect the shape of the distribution. This can be a little bit unintuitive, and there are other parameterizations of the generalized hyperbolic distribution that might feel more natural.

## Numerical Parameter Estimation

Just as we did in the last two posts, we can use the maximum likelihood method to fit a generalized hyperbolic distribution to a given set of data. Here is the likelihood function that we want to maximize in this case:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    L(\mu, \delta, \alpha, \lambda \mid \mathbf{x})
    =
    \prod_{i = 1}^{n} f(x_i \mid \mu, \delta, \alpha, \lambda)
    \end{displaymath}
    \end{document}
{% endlatex %}

To fit a generalized hyperbolic distribution to an observed set of data, we need to find the parameter values that maximize this function. As we did in the previous posts, we can use a numerical optimization method to find the most optimal parameter values. And to use a numerical optimization method, we need a cost function:

{% latex fig-11%}
    \begin{document}
    \begin{displaymath}
    C(\mu, \delta, \alpha, \lambda)
    =
    -\sum_{i = 1}^{n} \log\2\brace2[]{ f(x_i \mid \mu, \delta, \alpha, \lambda) }
    \end{displaymath}
    \end{document}
{% endlatex %}

This cost function is the negation of the logarithm of the likelihood function. We use the negation because, in the examples in the following sections, the numerical optimization method we use is a third-party implementation that finds minimums instead of maximums. The optimization method needs to start with an initial guess. Here is the initial value we're going to use for the subclass parameter:

{% latex fig-12%}
    \begin{document}
    \begin{displaymath}
    \lambda_0 = -0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

For the remaining parameters, we can use initial values based on the mean and standard deviation values that would conform to a normal distribution:

{% latex fig-13%}
    \begin{document}
    \begin{displaymath}
    \mu_0 = \frac{1}{n} \sum_{i = 1}^{n} x_i
    , \quad
    \delta_0 = \sqrt{\displaystyle \frac{1}{n} \sum_{i = 1}^{n} \2 (x_i - \mu_0)^2}
    , \quad
    \alpha_0 = \frac{1}{\delta_0}
    \end{displaymath}
    \end{document}
{% endlatex %}

These initial values seem to work pretty well for the data sets used in the following sections. While numerical optimization appears to be the simplest approach to finding the optimal parameter values, it might be possible to come up with an analytic solution if you're willing to work out the tedious mathematics. But for now, I think it's more convenient to use numerical methods.

## Microsoft Stock Prices

Now let's see how well the generalized hyperbolic distribution fits some real-life data. As we did in the last two posts, we'll use the historical stock prices of Microsoft Corporation for our first example. And like before, we'll take the first differences of the logarithm of the daily closing prices and plot the data in a histogram. The following charts show the histogram overlaid with the probability density functions for a fitted normal distribution, a generalized hyperbolic distribution using the initial parameter values, and a fitted generalized hyperbolic distribution with the optimized parameter values, respectively:

{% chart fig-14-fitted-MSFT-N.svg %}
{% chart fig-15-fitted-MSFT-I.svg %}
{% chart fig-16-fitted-MSFT-H.svg %}

The fitted generalized hyperbolic distribution has the following subclass parameter:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \lambda = -0.9748
    \end{displaymath}
    \end{document}
{% endlatex %}

This seems like a pretty good fit. And it might be an even better fit if we had allowed a bit of skew. Without measuring it objectively, I'd say it's probably a better fit than the stable distribution or the generalized normal distribution studied in previous posts.

## Bitcoin Prices

The next example uses historical bitcoin prices. We'll do the same analysis we did with the previous example. The following charts show the histogram overlaid with the probability density functions for a fitted normal distribution, a generalized hyperbolic distribution using the initial parameter values, and a fitted generalized hyperbolic distribution with the optimized parameter values, respectively:

{% chart fig-18-fitted-BTCUSD-N.svg %}
{% chart fig-19-fitted-BTCUSD-I.svg %}
{% chart fig-20-fitted-BTCUSD-H.svg %}

The fitted generalized hyperbolic distribution has the following subclass parameter:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    \lambda = -0.0696
    \end{displaymath}
    \end{document}
{% endlatex %}

Again, this looks like a good fit. Subjectively speaking, it looks like a much better fit than the fitted stable distribution in the last post. And I think it looks like a better fit than the generalized normal distribution studied previously as well.

## Natural Gas Prices

For the third example, let's use the historical closing prices of a natural gas ETF. Once again, we'll create a histogram based on the price data. The following charts show the histogram overlaid with the probability density functions for a fitted normal distribution, a generalized hyperbolic distribution using the initial parameter values, and a fitted generalized hyperbolic distribution with the optimized parameter values, respectively:

{% chart fig-22-fitted-UNG-N.svg %}
{% chart fig-23-fitted-UNG-I.svg %}
{% chart fig-24-fitted-UNG-H.svg %}

The fitted generalized hyperbolic distribution has the following subclass parameter:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    \lambda = -2.5018
    \end{displaymath}
    \end{document}
{% endlatex %}

It appears to be another good fit. Without making objective measurements, it's hard to tell if this is a better fit than a fitted stable distribution. However, the shape of this distribution seems to be a better match than the shape of the generalized normal distribution.

## Further Study

I am quite pleased with the results of this experiment. For modeling price fluctuations in financial markets, the generalized hyperbolic distribution seems to be a better model than the stable distribution or the generalized normal distribution. This distribution has many interesting properties that might be worth studying further. A particularly interesting resource on this topic is a dissertation titled [*The Generalized Hyperbolic Model: Estimation, Financial Derivatives, and Risk Measures*](https://d-nb.info/961152192/34) by Karsten Prause. This might be a good reference to consult if you're interested in a deeper study on this subject matter.

{% accompanying_src_link %}
