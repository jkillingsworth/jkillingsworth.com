---
layout: post
title: Least Squares and Normal Distributions
---

The method of least squares estimates the coefficients of a model function by minimizing the sum of the squared errors between the model and the observed values. In this post, I show the derivation of the parameter estimates for a linear model. In addition, I show that the maximum likelihood estimation is the same as the least squares estimation when we assume the errors are normally distributed.

<!--excerpt-->

## Least Squares Estimation

Suppose we have a set of two-dimensional data points that we observed by measuring some kind of phenomenon:

<figure class="equation">
  {% latex 01 %}
    \{\ (x_1, y_1), (x_2, y_2), \dots, (x_n, y_n)\ \}
  {% endlatex %}
</figure>

Also suppose that the measuring device is inaccurate. The data we observed on one of the axes contains errors. Despite the errors, we know the correct readings fall somewhere on a line given by the following linear equation:

<figure class="equation">
  {% latex 02 %}
    \hat{y} = a_0 + a_1 x
  {% endlatex %}
</figure>

For each data point, we can compute the error as the difference between the observed value and the correct value according to the model function:

<figure class="equation">
  {% latex 03 %}
    \varepsilon_i = y_i - \hat{y}_i
  {% endlatex %}
</figure>

The errors can be positive or negative. Taking the square of each error always yields a positive number. We can define the sum of the squared errors like this:

<figure class="equation">
  {% latex 04 %}
    \begin{aligned}
    S &= \sum_{i = 1}^{n} {\varepsilon_i^2}
    \\[1em]
      &= \sum_{i = 1}^{n} {(y_i - \hat{y}_i)^2}
    \\[1em]
      &= \sum_{i = 1}^{n} {(y_i - a_0 - a_1 x_i)^2}
    \end{aligned}
  {% endlatex %}
</figure>

Since the coefficients are unknown variables, we can treat the sum of the squared errors as a function of the coefficients:

<figure class="equation">
  {% latex 05 %}
    S(a_0, a_1) = \textstyle \sum{(y_i - a_0 - a_1 x_i)^2}
  {% endlatex %}
</figure>

To estimate the coefficients of the model function using the least squares method, we need to figure out what values for the coefficients give us the smallest value for the sum of the squared errors. We can find the minimum by first taking the partial derivative of the sum of squares function with respect to each of the coefficients, setting the derivative to zero, and then solving for the coefficient. Here are the derivatives with respect to each coefficient:

<figure class="equation">
  {% latex 06 %}
    \begin{aligned}
    \frac{\partial S}{\partial a_0}
    &=
    -2 \textstyle \sum{(y_i - a_0 - a_1 x_i)}
    \\[1em]
    \frac{\partial S}{\partial a_1}
    &=
    -2 \textstyle \sum{(x_i y_i - a_0 x_i - a_1 x_i^2)}
    \end{aligned}
  {% endlatex %}
</figure>

Setting the derivative with respect to the first coefficient to zero, we get the following result:

<figure class="equation">
  {% latex 07 %}
    \begin{aligned}
    &
    \text{Let } \frac{\partial S}{\partial a_0} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 &= \textstyle \sum{(y_i - a_0 - a_1 x_i)}
    \\[1em]
      &= \textstyle \sum{y_i} - n a_0 - a_1 \sum{x_i}
    \end{aligned}
    \end{aligned}
  {% endlatex %}
</figure>

Rearranging the equation and solving for the coefficient:

<figure class="equation">
  {% latex 08 %}
    a_0 = \frac{\sum{y_i} - a_1 \sum{x_i}}{n}
  {% endlatex %}
</figure>

Setting the derivative with respect to the second coefficient to zero, we get the following result:

<figure class="equation">
  {% latex 09 %}
    \begin{aligned}
    &
    \text{Let } \frac{\partial S}{\partial a_1} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 &= \textstyle \sum{(x_i y_i - a_0 x_i - a_1 x_i^2)}
    \\[1em]
      &= \textstyle \sum{x_i y_i} - a_0 \sum{x_i} - a_1 \sum{x_i^2}
    \end{aligned}
    \end{aligned}
  {% endlatex %}
</figure>

Rearranging the equation and solving for the coefficient:

<figure class="equation">
  {% latex 10 %}
    a_1 = \frac{\sum{x_i y_i} - a_0 \sum{x_i}}{\sum{x_i^2}}
  {% endlatex %}
</figure>

Each one of the coefficients is given in terms of the other. Since there are two equations and two unknowns, you can plug one into the other to derive the final outcome. Another way to do this might be to treat the results as a system of linear equations arranged as follows:

<figure class="equation">
  {% latex 11 %}
    \begin{aligned}
    \textstyle (n) a_0 + (\sum{x_i^{\phantom{1}}}) a_1
    &=
    \textstyle \sum{y_i}
    \\[1em]
    \textstyle (\sum{x_i^{\phantom{1}}}) a_0 + (\sum{x_i^2}) a_1
    &=
    \textstyle \sum{x_i y_i}
    \end{aligned}
  {% endlatex %}
</figure>

The coefficients can then be found by solving the following matrix equation:

<figure class="equation">
  {% latex 12 %}
    \left[\begin{array}{lll}
    a_0 \\[1em]
    a_1 \\
    \end{array}\right]
    =
    \left[\begin{array}{lll}
    n             & \sum{x_i^{ }} \\[1em]
    \sum{x_i^{ }} & \sum{x_i^{2}} \\
    \end{array}\right]
    ^{-1}
    \left[\begin{array}{lll}
    \sum{y_i^{ }}                   \\[1em]
    \sum{x_i^{\phantom{1}} y_i^{ }} \\
    \end{array}\right]
  {% endlatex %}
</figure>

This method is perhaps a cleaner approach. It can also work well for model functions with many coefficients, such as higher order polynomials or multivariable functions.

## Maximum Likelihood Estimation

Now let's assume the errors are normally distributed. That is to say, the observed values are normally distributed around the model. The probability density function for the normal distribution looks like this:

<figure class="equation">
  {% latex 13 %}
    f(y \mid \hat{y}, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp \left[- \frac{(y - \hat{y})^2}{2 \sigma^2} \right]
  {% endlatex %}
</figure>

Here we treat our model as the mean. We also consider the standard deviation, depicted by sigma, which measures the spread of the data around the mean. Given our observed data points, we want to figure out what the most likely values are for the mean and standard deviation. For a single data point alone, the likelihood function for a given mean and standard deviation is:

<figure class="equation">
  {% latex 14 %}
    L(\hat{y}, \sigma \mid y)
    =
    f(y \mid \hat{y}, \sigma)
  {% endlatex %}
</figure>

The likelihood is equal to the probability density. For all data points combined, the likelihood function for a given mean and standard deviation is equal to the product of the density for each data point individually:

<figure class="equation">
  {% latex 15 %}
    L(\hat{y}, \sigma \mid y_1, y_2, \dots, y_n)
    =
    \prod_{i = 1}^{n}{f(y_i \mid \hat{y}_i, \sigma)}
  {% endlatex %}
</figure>

At this point, we just need to find the mean and standard deviation values that maximize the likelihood function. Similar to what we did in the previous section, we can find the maximum by first taking the partial derivative of the likelihood function with respect to each of the coefficients, setting the derivative to zero, and then solving for the coefficients. This might be easier to do if we first take the natural logarithm of the likelihood function:

<figure class="equation">
  {% latex 16 %}
    \begin{aligned}
    \ln{L}
    &=
    \sum_{i = 1}^{n}{\ln{f(y_i \mid \hat{y}_i, \sigma)}}
    \\[1em]
    &=
    \sum_{i = 1}^{n}
    {\left[
    - \ln{\left(\sigma \sqrt{2 \pi} \right)}
    - \frac{(y_i - \hat{y}_i)^2}{2 \sigma^2}
    \right]}
    \\[1em]
    &=
    \sum_{i = 1}^{n}
    {\left[
    - \ln{(\sigma)}
    - \frac{1}{2} \ln{({2 \pi})}
    - \frac{(y_i - \hat{y}_i)^2}{2 \sigma^2}
    \right]}
    \\[1em]
    &=
    - n \ln{(\sigma)}
    - \frac{n}{2} \ln{(2 \pi)}
    - \frac{1}{2 \sigma^2} \sum_{i = 1}^{n} {(y_i - \hat{y}_i)^2}
    \end{aligned}
  {% endlatex %}
</figure>

Since we're interested in finding the coefficients of the model function, we can replace the mean parameter with the body of the model function and treat the likelihood function as a function of the coefficients:

<figure class="equation">
  {% latex 17 %}
    \ln{L(a_0, a_1, \sigma)}
    =
    - n \ln{(\sigma)}
    - \frac{n}{2} \ln{(2 \pi)}
    - \frac{1}{2 \sigma^2} \textstyle \sum{(y_i - a_0 - a_1 x_i)^2}
  {% endlatex %}
</figure>

Let's call this the log-likelihood function. Since the natural logarithm function is monotonically increasing function, we can maximize the log-likelihood function and get the same result we would get if we maximized the original likelihood function. Here are the partial derivatives of the log-likelihood function with respect to each of the coefficients:

<figure class="equation">
  {% latex 18 %}
    \begin{aligned}
    \frac{\partial \ln{L}}{\partial a_0}
    &=
    \frac{1}{\sigma^2} \textstyle \sum{(y_i - a_0 - a_1 x_i)}
    \\[1em]
    \frac{\partial \ln{L}}{\partial a_1}
    &=
    \frac{1}{\sigma^2} \textstyle \sum{(x_i y_i - a_0 x_i - a_1 x_i^2)}
    \end{aligned}
  {% endlatex %}
</figure>

Setting the derivative with respect to the first coefficient to zero, we get the following result:

<figure class="equation">
  {% latex 19 %}
    \begin{aligned}
    &
    \text{Let } \frac{\partial \ln{L}}{\partial a_0} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 &= \textstyle \sum{(y_i - a_0 - a_1 x_i)}
    \\[1em]
      &= \textstyle \sum{y_i} - n a_0 - a_1 \sum{x_i}
    \end{aligned}
    \end{aligned}
  {% endlatex %}
</figure>

Rearranging the equation and solving for the coefficient:

<figure class="equation">
  {% latex 20 %}
    a_0 = \frac{\sum{y_i} - a_1 \sum{x_i}}{n}
  {% endlatex %}
</figure>

Setting the derivative with respect to the second coefficient to zero, we get the following result:

<figure class="equation">
  {% latex 21 %}
    \begin{aligned}
    &
    \text{Let } \frac{\partial \ln{L}}{\partial a_1} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 &= \textstyle \sum{(x_i y_i - a_0 x_i - a_1 x_i^2)}
    \\[1em]
      &= \textstyle \sum{x_i y_i} - a_0 \sum{x_i} - a_1 \sum{x_i^2}
    \end{aligned}
    \end{aligned}
  {% endlatex %}
</figure>

Rearranging the equation and solving for the coefficient:

<figure class="equation">
  {% latex 22 %}
    a_1 = \frac{\sum{x_i y_i} - a_0 \sum{x_i}}{\sum{x_i^2}}
  {% endlatex %}
</figure>

As you can see, we get at the same results we got from using the method of least squares to estimate the coefficients. For completeness, the same procedure can be used to find the standard deviation:

<figure class="equation">
  {% latex 23 %}
    \begin{aligned}
    \frac{\partial \ln{L}}{\partial \sigma}
    &=
    - \frac{n}{\sigma}
    + \frac{1}{\sigma^3} \textstyle \sum{(y_i - a_0 - a_1 x_i)^2}
    \end{aligned}
  {% endlatex %}
</figure>

Setting the derivative to zero, we get the following result for the standard deviation:

<figure class="equation">
  {% latex 24 %}
    \begin{aligned}
    &
    \text{Let } \frac{\partial \ln{L}}{\partial \sigma} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 &= - \frac{n}{\sigma} + \frac{1}{\sigma^3} \textstyle \sum{(y_i - a_0 - a_1 x_i)^2}
    \\[1em]
      &= - n \sigma^2 + \textstyle \sum{(y_i - a_0 - a_1 x_i)^2}
    \end{aligned}
    \end{aligned}
  {% endlatex %}
</figure>

Rearranging the equation and solving for sigma:

<figure class="equation">
  {% latex 25 %}
    \sigma = \sqrt{\frac{1}{n} \textstyle \sum{(y_i - a_0 - a_1 x_i)^2}}
  {% endlatex %}
</figure>

Note that this result may yield a biased estimate of the standard deviation when computing the value based on a limited number of samples. It might be more appropriate to use an unbiased estimator that takes the number of degrees of freedom into consideration. But that's out of scope for this post. Perhaps it's a topic I'll explore at another time.
