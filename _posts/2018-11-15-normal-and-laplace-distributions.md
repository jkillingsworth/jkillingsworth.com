---
layout: post
title: Normal and Laplace Distributions
---

I'm interested in studying the Laplace distribution. I was once under the impression that price fluctuations in the financial markets were normally distributed. However, as I plan to show in a [later post]({% post_url 2019-01-26-the-distribution-of-price-fluctuations %}), stock prices seem to move up and down according to a Laplace distribution instead. Before analyzing any historical price data, I first want to lay some groundwork and compare the Laplace distribution to the normal distribution.

<!--excerpt-->

## The Normal Distribution

Suppose we have a continuous random variable whose possible values are distributed according to a normal distribution. The probability density function is:

{% latex 1 fig-01 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp \2 \brace4[]{- \frac{(x - \mu)^2}{2 \sigma^2}}
    \end{displaymath}
    \end{document}
{% endlatex %}

If we have some samples of a random variable that we expect to have a normal distribution, we can estimate the parameters of the density function using the maximum likelihood method described in some of my [previous posts]({% post_url 2018-10-11-least-squares-and-normal-distributions %}#maximum-likelihood-estimation). Since it's more convenient in this case, instead of maximizing the likelihood function, let's maximize the logarithm of the likelihood function:

{% latex 1 fig-02 %}
    \begin{document}
    \begin{displaymath}
    \ln L(\mu, \sigma)
    =
    - n \ln \2 (\sigma)
    - \frac{n}{2} \ln \2 (2 \pi)
    - \frac{1}{2 \sigma^2} \sum_{i = 1}^{n} \2 (x_i - \mu)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to know what values for the mean and standard deviation parameters have the highest possible likelihood. To do that, we can figure out where the derivative of the log-likelihood function with respect to each of the parameters is equal to zero. Here is the partial derivative of the log-likelihood function with respect to the mean:

{% latex 1 fig-03 %}
    \begin{document}
    \begin{displaymath}
    \frac{\pderiv \ln L}{\pderiv \mu}
    =
    \frac{1}{\sigma^2} \sum_{i = 1}^{n} \2 (x_i - \mu)
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the partial derivative to zero and solving for the mean, we arrive at the following estimated value:

{% latex 1 fig-04 %}
    \begin{document}
    \begin{displaymath}
    \hat{\mu} = \frac{1}{n} \sum_{i = 1}^{n} x_i
    \end{displaymath}
    \end{document}
{% endlatex %}

Once we have the value for the mean, we can follow the same steps to solve for the standard deviation. Here is the partial derivative of the log-likelihood function with respect to the standard deviation:

{% latex 1 fig-05 %}
    \begin{document}
    \begin{displaymath}
    \frac{\pderiv \ln L}{\pderiv \sigma}
    =
    - \frac{n}{\sigma}
    + \frac{1}{\sigma^3} \sum_{i = 1}^{n} \2 (x_i - \mu)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the partial derivative to zero and solving for the standard deviation, we get this estimated value:

{% latex 1 fig-06 %}
    \begin{document}
    \begin{displaymath}
    \hat{\sigma} = \sqrt{\displaystyle \frac{1}{n} \sum_{i = 1}^{n} \2 (x_i - \mu)^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

If you want to see a more detailed breakdown of the steps above, you can reference my post titled [*Least Squares and Normal Distributions*]({% post_url 2018-10-11-least-squares-and-normal-distributions %}). As I mentioned in that post, the maximum likelihood estimator for the standard deviation can give an estimate that is too low for small sample sizes. If using a limited sample size, it might be a good idea to apply [Bessel's correction](https://en.wikipedia.org/wiki/Bessel%27s_correction) to get a more accurate estimate.

## The Laplace Distribution

Suppose we have a continuous random variable whose possible values are distributed according to a Laplace distribution. The probability density function is:

{% latex 1 fig-07 %}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, b)
    =
    \frac{1}{2b} \exp \2 \brace3(){- \frac{|x - \mu|}{b}}
    \end{displaymath}
    \end{document}
{% endlatex %}

If we have a set of samples of a random variable that we know to have a Laplace distribution, we can estimate the parameters using the same approach we took for estimating the parameters of the normal distribution. We can use the maximum likelihood method. Here is the log-likelihood function we want to maximize:

{% latex 1 fig-08 %}
    \begin{document}
    \begin{displaymath}
    \ln L(\mu, b)
    =
    - n \ln \2 (2b)
    - \frac{1}{b} \sum_{i = 1}^{n} |x_i - \mu|
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to know what values of the location and scale parameters have the greatest likelihood. The analytical approach is to take the derivative, set it to zero, and solve for the parameters. But consider the absolute value function:

{% latex 1 fig-09 %}
    \begin{document}
    \begin{displaymath}
    |x - \mu|
    =
    \begin{dcases}
    \mu - x & \quad \text{if $x < \mu$}
    \\
    x - \mu & \quad \text{if $x > \mu$}
    \\
    0       & \quad \text{if $x = \mu$}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

It's a piecewise function. Taking the derivative of the log-likelihood function with respect to the location parameter can be a bit tricky because the absolute value function, although continuous, is not differentiable at all points:

{% latex 1 fig-10 %}
    \begin{document}
    \begin{displaymath}
    \frac{\dderiv}{\dderiv \mu} |x - \mu|
    =
    \begin{dcases}
    -1               & \quad \text{if $x < \mu$}
    \\
    +1               & \quad \text{if $x > \mu$}
    \\
    \text{undefined} & \quad \text{if $x = \mu$}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

To be more succinct, we can represent the derivative of the absolute value function using the sign function:

{% latex 1 fig-11 %}
    \begin{document}
    \begin{displaymath}
    \frac{\dderiv}{\dderiv \mu} |x - \mu|
    =
    \operatorname{sgn} \2 (x - \mu), \quad x \neq \mu
    \end{displaymath}
    \end{document}
{% endlatex %}

The sign function simply returns the sign of a value:

{% latex 1 fig-12 %}
    \begin{document}
    \begin{displaymath}
    \operatorname{sgn} \2 (x - \mu)
    =
    \begin{dcases}
    -1              & \quad \text{if $x < \mu$}
    \\
    +1              & \quad \text{if $x > \mu$}
    \\
    \phantom{\pm} 0 & \quad \text{if $x = \mu$}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can express the partial derivative of the log-likelihood function with respect to the location parameter as:

{% latex 1 fig-13 %}
    \begin{document}
    \begin{displaymath}
    \frac{\pderiv \ln L}{\pderiv \mu}
    =
    \frac{1}{b} \sum_{i = 1}^{n} \operatorname{sgn} \2 (x_i - \mu), \quad x_i \neq \mu
    \end{displaymath}
    \end{document}
{% endlatex %}

This is really just giving us the number of samples with a value greater than the location parameter minus the number of samples with a value less than the location parameter. Note also that the derivative is undefined at points where the location parameter equals the value of one of the samples. While not adequate for an analytical solution, this does provide a clue that the best estimate is at or near the median value. Let's rank our samples in ascending order:

{% latex 1 fig-14 %}
    \begin{document}
    \begin{displaymath}
    \{\, x_1, x_2, \dots, x_m, \dots, x_{n-1}, x_n \mid x_i \leq x_{i+1} \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's also choose a middle value that is about halfway between the first and last sample in the ordered set. The exact value depends on whether the total number of samples is an even number or an odd number:

{% latex 1 fig-15 %}
    \begin{document}
    \begin{displaymath}
    m
    =
    \begin{dcases}
    \frac{n}{2}     & \quad \text{if $n$ is even}
    \\[0.5em]
    \frac{n + 1}{2} & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can glean some insights by looking at a plot of the likelihood value for possible values of the location parameter. When there is an even number of samples, the likelihood function looks like this:

{% chart fig-16-log-likelihood-laplace-e.svg %}

Notice that there is a range of possible values where the likelihood is at a maximum when there is an even number of samples. For an odd number of samples, the likelihood function looks slightly different:

{% chart fig-17-log-likelihood-laplace-o.svg %}

For an odd number of samples, there is a single point at which the likelihood is maximized. By inspection, we can conclude that the median value of our samples has the highest likelihood for the location parameter:

{% latex 1 fig-18 %}
    \begin{document}
    \begin{displaymath}
    \hat{\mu}
    =
    \begin{dcases}
    \frac{1}{2} (x_m + x_{m+1}) & \quad \text{if $n$ is even}
    \\[0.5em]
    x_m                         & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

If we have an even number of samples, we just take the mean of the two median values. Once an estimate of the location parameter is known, solving for the scale parameter is a bit easier since there is an analytical solution. Here is the partial derivative of the log-likelihood function with respect to the scale parameter:

{% latex 1 fig-19 %}
    \begin{document}
    \begin{displaymath}
    \frac{\pderiv \ln L}{\pderiv b}
    =
    - \frac{n}{b}
    + \frac{1}{b^2} \sum_{i = 1}^{n} |x_i - \mu|
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the partial derivative to zero and solving for the scale parameter, we get the following estimate:

{% latex 1 fig-20 %}
    \begin{document}
    \begin{displaymath}
    \hat{b} = \frac{1}{n} \sum_{i = 1}^{n} |x_i - \mu|
    \end{displaymath}
    \end{document}
{% endlatex %}

I think it's worth mentioning here that this method of estimating the parameters of a Laplace distribution doesn't sit well with me. Choosing the median value for the location parameter seems like a coarse approach. In cases where there is a range of possible values for the location, I wonder just how wide that range can be in practice. There might be other estimation techniques worth looking into, but I want to see how well this one works with real data before exploring alternatives.

## Comparison

The normal distribution and the Laplace distribution are both symmetrical. The density functions of each have a similar structure. And with a small number of samples, it might be difficult to determine if a random variable has a normal distribution or a Laplace distribution. However, there are some important differences that are best shown with an illustration:

{% chart fig-21-distributions-lin.svg %}

Both density functions have the same basic shape. The density plot of the Laplace distribution, however, is taller and skinnier in the middle. It also has fatter tails than the normal distribution. I think those fat tails are worth taking a closer look at. Here is the same chart with the density plotted on a logarithmic scale:

{% chart fig-22-distributions-log.svg %}

Notice the difference in magnitude for values far from the middle. The probability of observing a value of a normally distributed random variable far from the mean is quite small. The probability of observing the same value, while still small, might be orders of magnitude greater if the random variable has a Laplace distribution.

{% accompanying_src_link %}
