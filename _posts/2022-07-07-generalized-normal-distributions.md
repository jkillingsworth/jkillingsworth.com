---
layout: post
title: Generalized Normal Distributions
---

The generalized normal distribution is a family of probability distributions that vary according to a shape parameter. The symmetrical variant of this distribution may go by other names such as the generalized error distribution, the generalized Gaussian distribution, etc. In this post, we will explore this probability distribution and its relationship with the normal distribution and the Laplace distribution. I'll also show some examples illustrating the use of the maximum likelihood method to estimate the parameters of the distribution using real-life data.

<!--excerpt-->

## Probability Density Function

The generalized normal distribution is a continuous probability distribution with three parameters: a location parameter, a scale parameter, and a shape parameter. The probability density function takes the following form:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    &
    f(x \mid \mu, \alpha, \beta)
    =
    \frac{\beta}{2 \alpha \Gamma(\nicefrac{1}{\beta})}
    \exp\2\brace4[]{ -\brace3(){ \frac{|x - \mu|}{\alpha} }^\beta }
    \\[1em]
    &
    \begin{aligned}
    & \mu    && \text{is the location parameter}
    \\
    & \alpha && \text{is the scale parameter}
    \\
    & \beta  && \text{is the shape parameter}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The location parameter can be negative or positive. The scale parameter and the shape parameter are always positive real numbers. Note the use of the [gamma function](https://en.wikipedia.org/wiki/Gamma_function) above. The gamma function looks like this:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \Gamma(z)
    =
    \int_{0}^{\infty} x^{z - 1} e^{-x} \, \dderiv x
    \end{displaymath}
    \end{document}
{% endlatex %}

For practical purposes, we can just use a numerical method to approximate the gamma function. I am using a third-party implementation of the [Lanczos approximation](https://en.wikipedia.org/wiki/Lanczos_approximation) for the illustrations in this post. If we hold the location and scale parameters constant and then vary the shape parameter, we can see what the shape of the density function looks like for different values of the shape parameter. Here are some illustrations:

{% chart fig-03-model-shape-0.50.svg %}
{% chart fig-04-model-shape-0.75.svg %}
{% chart fig-05-model-shape-1.00.svg %}
{% chart fig-06-model-shape-1.50.svg %}
{% chart fig-07-model-shape-2.00.svg %}
{% chart fig-08-model-shape-4.00.svg %}
{% chart fig-09-model-shape-8.00.svg %}

If you think the density function looks like that of a Laplace distribution when the shape parameter is equal to one, then you would be correct. And if you think the density function looks like that of a normal distribution when the shape parameter is equal to two, then you would be correct again. Indeed, both the normal distribution and the Laplace distribution are special cases of the generalized normal distribution. The generalized normal distribution can also take the form of a uniform distribution as the shape parameter approaches infinity.

## Normal Distribution

The normal distribution is a special case of the generalized normal distribution when the shape parameter is equal to two. Consider the following:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \beta = 2, \quad \Gamma(\nicefrac{1}{2}) = \sqrt{\pi}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plug these values into the density function and replace the scale parameter with the following:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    \alpha = \sigma \sqrt{2}
    \end{displaymath}
    \end{document}
{% endlatex %}

We now have a familiar representation of the normal distribution:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp\2\brace4[]{ -\frac{(x - \mu)^2}{2 \sigma^2} }
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, by holding the shape parameter to a fixed value of two, the generalized normal distribution can be treated like a regular normal distribution.

## Laplace Distribution

The Laplace distribution is a special case of the generalized normal distribution when the shape parameter is equal to one. Consider the following:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    \beta = 1, \quad \Gamma(\nicefrac{1}{1}) = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Plug these values into the density function and replace the scale parameter with the following:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    \alpha = b
    \end{displaymath}
    \end{document}
{% endlatex %}

We now have a familiar representation of the Laplace distribution:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, b)
    =
    \frac{1}{2b} \exp\2\brace3(){ -\frac{|x - \mu|}{b} }
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, by holding the shape parameter to a fixed value of one, the generalized normal distribution can be treated like a Laplace distribution.

## Numerical Parameter Estimation

If you have a set of observed data that is distributed according to a known probability distribution, you can use the maximum likelihood method to estimate the parameters of the distribution. If the distribution is a normal distribution or a Laplace distribution, the parameter values can be solved for analytically by taking the partial derivative of the likelihood function with respect to each one of the parameters. You can reference my earlier post titled [*Normal and Laplace Distributions*]({% post_url 2018-11-15-normal-and-laplace-distributions %}) for a deeper explanation. But what if taking the derivative is difficult or impossible to do? Consider the likelihood function for the generalized normal distribution:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    L(\mu, \alpha, \beta \mid \mathbf{x})
    =
    \prod_{i = 1}^{n} f(x_i \mid \mu, \alpha, \beta)
    \end{displaymath}
    \end{document}
{% endlatex %}

To fit the generalized normal distribution to an observed set of data, we need to find the parameter values that maximize this function. Instead of coming up with an analytical solution, we can use a numerical optimization method. Taking this approach, we need to come up with a cost function that our optimization method can evaluate iteratively. Here is the cost function that we will use in the examples in the following sections:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    C(\mu, \alpha, \beta)
    =
    -\sum_{i = 1}^{n} \log\2\brace2[]{ f(x_i \mid \mu, \alpha, \beta) }
    \end{displaymath}
    \end{document}
{% endlatex %}

This is just the negation of the logarithm of the likelihood function. We want to take the negative in this case because, in the examples in the following sections, we're going to use an implementation of the Nelder--Mead optimization method that finds minimums instead of maximums. And by using the logarithm of the likelihood function, we can avoid dealing with numbers that are too large for a double-precision floating-point number. Since our chosen optimization method requires an initial guess of the parameter values, we can start by giving the shape parameter a value of two:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    \beta_0 = 2
    \end{displaymath}
    \end{document}
{% endlatex %}

This would imply a normal distribution, so we might also set the initial guess for the location and scale parameters accordingly:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    \mu_0 = \frac{1}{n} \sum_{i = 1}^{n} x_i
    , \quad
    \alpha_0 = \sqrt{\displaystyle \frac{2}{n} \sum_{i = 1}^{n} \2 (x_i - \mu_0)^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

This puts our initial parameter estimates in the right ballpark. Our numerical optimization algorithm can then iteratively find increasingly better and better estimates until some terminating criterion is met. If we know that the distribution of our data more closely resembles that of a Laplace distribution---as with the data studied in my post titled [*The Distribution of Price Fluctuations*]({% post_url 2019-01-26-the-distribution-of-price-fluctuations %})---we might choose an initial guess based on the parameters fitted to a Laplace distribution instead. The numerical approximation should come out roughly the same in either case.

## Microsoft Stock Prices

Now let's take a look at some examples of fitting the generalized normal distribution to some data in the wild. For this example, we'll use the historical stock prices of Microsoft Corporation going back to 1986. We'll take the logarithm of the daily closing prices, compute the first differences, and then put the data in a histogram. The following charts show the histogram overlaid with the fitted normal, Laplace, and generalized normal density functions, respectively:

{% chart fig-20-fitted-MSFT-N.svg %}
{% chart fig-21-fitted-MSFT-L.svg %}
{% chart fig-22-fitted-MSFT-G.svg %}

The fitted generalized normal distribution has the following shape parameter:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    \beta = 0.9892
    \end{displaymath}
    \end{document}
{% endlatex %}

This value is very close to one, meaning that the shape of the density function is very close to that of the Laplace distribution. Eyeballing the charts above, you can't really tell the difference between the fitted Laplace density function and the fitted generalized normal density function.

## Bitcoin Prices

Let's do another example. This one uses historical bitcoin prices going back to 2011. Like before, we'll take the logarithm of the daily prices, compute the first differences, and then put the data in a histogram. The following charts show the histogram overlaid with the fitted normal, Laplace, and generalized normal density functions, respectively:

{% chart fig-24-fitted-BTCUSD-N.svg %}
{% chart fig-25-fitted-BTCUSD-L.svg %}
{% chart fig-26-fitted-BTCUSD-G.svg %}

The fitted generalized normal distribution has the following shape parameter:

{% latex fig-27 %}
    \begin{document}
    \begin{displaymath}
    \beta = 0.7030
    \end{displaymath}
    \end{document}
{% endlatex %}

This is a bit smaller than the shape parameter that would conform to a Laplace distribution. As you can see in the charts above, the fitted density function for the generalized normal distribution is taller and thinner than the density function for the Laplace distribution.

## Natural Gas Prices

For the third example, let's use the historical prices of a natural gas ETF. As with the previous two examples, we'll take the logarithm of the daily price quotes, compute the first differences, and then put the data in a histogram. The following charts show the histogram overlaid with the fitted normal, Laplace, and generalized normal density functions, respectively:

{% chart fig-28-fitted-UNG-N.svg %}
{% chart fig-29-fitted-UNG-L.svg %}
{% chart fig-30-fitted-UNG-G.svg %}

The fitted generalized normal distribution has the following shape parameter:

{% latex fig-31 %}
    \begin{document}
    \begin{displaymath}
    \beta = 1.2923
    \end{displaymath}
    \end{document}
{% endlatex %}

This value is a bit larger than the shape parameter that would conform to a Laplace distribution. In contrast to the previous example, the fitted density function for the generalized normal distribution in this example is shorter and wider than the density function for the Laplace distribution.

## Other Distributions

For the data sets used in the examples above (and for similar data sets representing price fluctuations in financial markets), there is no doubt that fitting the data to a generalized normal distribution gives better results than fitting the data to a Laplace distribution. But I am not convinced that this is the best kind of probability distribution to use for modeling this type of data. In each of the examples above, the peak of the distribution implied by the histogram seems to be much more rounded than the density function of the fitted generalized normal distribution. Perhaps a [Cauchy distribution](https://en.wikipedia.org/wiki/Cauchy_distribution) might be a better alternative. And perhaps the numerical techniques used here could open the door to exploring the use of [other types](https://en.wikipedia.org/wiki/Stable_distribution) of probability distributions as well.

{% accompanying_src_link %}
