---
layout: post
title: Understanding the Discrete Fourier Transform
---

For the longest time, the Fourier transform remained a bit of a mystery to me. I knew it involved transforming a function in the time domain to a representation in the frequency domain. And I knew it had something to do with sinusoidal waves. But I didn't understand what it meant to have a frequency domain representation of a function. As it turns out, it's quite a simple thing once you realize what the frequency values represent. In this post, I explain the discrete Fourier transform by working through a set of examples.

<!--excerpt-->

## The Forward Transform and the Inverse Transform

Suppose we have a function in the time domain. The value varies as a function of time. Let's assume we can represent this function as a continuous sum of a potentially infinite number of sinusoidal waves of various frequencies, amplitudes, and phase shifts. The Fourier transform lets us decompose this function into its sinusoidal components on the frequency spectrum:

{% latex 1 fig-01 %}
    \begin{document}
    \begin{displaymath}
    \mathcal{F}^{\,+1\!} \brace1{\lbrace}{\rbrace}{ x(t) }
    =
    y(f)
    =
    \int_{-\infty}^{\infty} x(t) \, e^{-i 2 \pi f t} \, \dderiv t
    \end{displaymath}
    \end{document}
{% endlatex %}

The formula above is what we can think of as the forward transform. It takes a function in the time domain and gives us back a function in the frequency domain. We can also take a function in the frequency domain and map it back to the time domain using the inverse transform:

{% latex 1 fig-02 %}
    \begin{document}
    \begin{displaymath}
    \mathcal{F}^{\,-1\!} \brace1{\lbrace}{\rbrace}{ y(f) }
    =
    x(t)
    =
    \int_{-\infty}^{\infty} y(f) \, e^{i 2 \pi f t} \, \dderiv f
    \end{displaymath}
    \end{document}
{% endlatex %}

The forward and inverse Fourier transforms above operate on continuous functions. The inputs are assumed to be continuous functions across the time domain and frequency spectrum, respectively. In practical scenarios, however, we are more likely to be dealing with a set of discrete samples taken at regular intervals. And for this, we need to focus our attention on the discrete Fourier transform. Here is the forward transform:

{% latex 1 fig-03 %}
    \begin{document}
    \begin{displaymath}
    y_k = y(f_k) = \frac{1}{N} \sum_{n = 0}^{N - 1} x(t_n) \, e^{-i 2 \pi f_k t_n}
    \end{displaymath}
    \end{document}
{% endlatex %}

Instead of an integral, we have a sum. And the sum is based on discrete samples of the function in the time domain. Notice that the complex exponential kernel is still the same as before. Naturally, we have a corresponding inverse transform that goes with it:

{% latex 1 fig-04 %}
    \begin{document}
    \begin{displaymath}
    x_n = x(t_n) = \sum_{k = 0}^{N - 1} y(f_k) \, e^{i 2 \pi f_k t_n}
    \end{displaymath}
    \end{document}
{% endlatex %}

I think the discrete transform is more intuitive and easier to understand. We'll work through some examples in the following sections. I want to point out here that the notation I'm using might differ somewhat from the notation conventions used in textbooks and other resources on this topic. To each their own. For each of our examples, we're going to use a sampling frequency of eight samples per second in the time domain:

{% latex 1 fig-05 %}
    \begin{document}
    \begin{displaymath}
    N = 8
    \end{displaymath}
    \end{document}
{% endlatex %}

We're only going to take samples over a finite time window of one second. That means we're going to have a total of eight data points. The following table shows the discrete points in time at which we'll sample the input function:

{% latex 1 fig-06 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|}
    \hline
    n & t_n
    \\[0.25em]\hline
    0 & 0.0000
    \\[0.25em]\hline
    1 & 0.1250
    \\[0.25em]\hline
    2 & 0.2500
    \\[0.25em]\hline
    3 & 0.3750
    \\[0.25em]\hline
    4 & 0.5000
    \\[0.25em]\hline
    5 & 0.6250
    \\[0.25em]\hline
    6 & 0.7500
    \\[0.25em]\hline
    7 & 0.8750
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to round trip our input function from the time domain to the frequency domain and then back again to the time domain. We need to choose some discrete frequencies. Here are the eight different frequencies we'll use:

{% latex 1 fig-07 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|}
    \hline
    k & f_k
    \\[0.25em]\hline
    0 & 0.0000
    \\[0.25em]\hline
    1 & 1.0000
    \\[0.25em]\hline
    2 & 2.0000
    \\[0.25em]\hline
    3 & 3.0000
    \\[0.25em]\hline
    4 & 4.0000
    \\[0.25em]\hline
    5 & 5.0000
    \\[0.25em]\hline
    6 & 6.0000
    \\[0.25em]\hline
    7 & 7.0000
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

We could have chosen a different set of discrete points on the time domain to sample the input function. Likewise, we don't necessarily have to use this specific set of frequencies. But these values seem to make the most sense in this context as they will likely yield the most intuitive results as we work through the examples in the following sections.

## Example 1

For our first example, let's just use a simple sine wave. And let's use a sine wave with a period of one second. That means it starts at zero and finishes a complete cycle precisely one second later. Here is the function definition:

{% latex 1 fig-08 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \sin\2\brace1(){2 \pi t}
    \end{displaymath}
    \end{document}
{% endlatex %}

To foreshadow what is to come, I think it's worth pointing out here that a sine wave can also be understood as a cosine wave in which the phase is shifted by a quarter of a period. Here is how you can define a sine wave using the cosine function:

{% latex 1 fig-09 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \cos\2\brace1(){2 \pi t - 0.5 \pi}
    \end{displaymath}
    \end{document}
{% endlatex %}

The two function definitions above are equivalent. Keep that in the back of your mind as you read further. And while this is a continuous function, we only want to perform our transform operations on discrete samples of the input function. Here are the sample values at each of the discrete points on the time domain:

{% latex 1 fig-10 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|wl{8em+2.0555em}|}
    \hline
    n & t_n    & x_n
    \\[0.25em]\hline
    0 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    1 & 0.1250 & +0.7071
    \\[0.25em]\hline
    2 & 0.2500 & +1.0000
    \\[0.25em]\hline
    3 & 0.3750 & +0.7071
    \\[0.25em]\hline
    4 & 0.5000 & \pm 0.0000
    \\[0.25em]\hline
    5 & 0.6250 & -0.7071
    \\[0.25em]\hline
    6 & 0.7500 & -1.0000
    \\[0.25em]\hline
    7 & 0.8750 & -0.7071
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Notice that at points where the function value is zero, the function might evaluate as either a positive zero or a negative zero. Now, in pure mathematics, there may be no such thing as a signed zero. But if you are working with a computer system that uses IEEE 754 floating-point numbers, there most certainly is such a thing as a signed zero. Just keep that in mind. Here is a plot of the sine wave function, along with the sample values:

{% chart fig-11-example-1-time-xs.svg %}

Once we have taken the sample values of the input function, we are ready to execute the discrete Fourier transform. Here is how to apply the forward transform:

{% latex 1 fig-12 %}
    \begin{document}
    \begin{displaymath}
    y(f) = \frac{1}{N} \sum_{n = 0}^{N - 1} x_n e^{-i 2 \pi f t_n}
    \end{displaymath}
    \end{document}
{% endlatex %}

This formula takes the form of a continuous function on the frequency domain. But what we really want to do here is take discrete samples of this function, one for each of our chosen frequencies. Here are the values:

{% latex 1 fig-13 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|wl{8em+2.0555em}|}
    \hline
    k & f_k    & y_k
    \\[0.25em]\hline
    0 & 0.0000 & \pm 0.0000 \pm i \0 0.0000
    \\[0.25em]\hline
    1 & 1.0000 & \pm 0.0000 - i \0 0.5000
    \\[0.25em]\hline
    2 & 2.0000 & \pm 0.0000 \pm i \0 0.0000
    \\[0.25em]\hline
    3 & 3.0000 & \pm 0.0000 \pm i \0 0.0000
    \\[0.25em]\hline
    4 & 4.0000 & \pm 0.0000 \pm i \0 0.0000
    \\[0.25em]\hline
    5 & 5.0000 & \pm 0.0000 \pm i \0 0.0000
    \\[0.25em]\hline
    6 & 6.0000 & \pm 0.0000 \pm i \0 0.0000
    \\[0.25em]\hline
    7 & 7.0000 & \pm 0.0000 + i \0 0.5000
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

The values on the frequency domain are complex numbers with both real and imaginary components. Again, keep in mind that zero values might be positive or negative when using a computer. The following plots show a visual representation for both axes of the complex plane:

{% chart fig-14-example-1-freq-ys-re.svg %}
{% chart fig-15-example-1-freq-ys-im.svg %}

The complex values above are represented using the Cartesian form. While this is a common way to express complex numbers, I think it makes more sense to use the polar form when discussing Fourier transforms. Consider the Cartesian form with distinct real and imaginary coefficients:

{% latex 1 fig-16 %}
    \begin{document}
    \begin{displaymath}
    y = a + ib
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to express this using the polar form:

{% latex 1 fig-17 %}
    \begin{document}
    \begin{displaymath}
    y = A \0 e^{i\phi}
    \end{displaymath}
    \end{document}
{% endlatex %}

The polar form of a complex number has coefficients for the magnitude and the phase angle. We can calculate these coefficients based on the real and imaginary parts used in the Cartesian form. The following formula shows how to compute the magnitude component:

{% latex 1 fig-18 %}
    \begin{document}
    \begin{displaymath}
    A = \sqrt{a^2 + b^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

Computing the phase angle is a bit more involved. If the complex value lies on the first or fourth quadrant of the complex plane, it's a simple arctangent calculation. But if it lies on the second or third quadrant or on the border between quadrants, we need to treat those values as special cases. Here is the formula for computing the phase angle:

{% latex 1 fig-19 %}
    \begin{document}
    \begin{displaymath}
    \phi =
    \begin{dcases}
    \arctan\2\brace1(){\tfrac{b}{a}}       & \quad \text{if $a > 0$}
    \\[0.5em]
    \arctan\2\brace1(){\tfrac{b}{a}} + \pi & \quad \text{if $a < 0$ and $b > 0$}
    \\[0.5em]
    \arctan\2\brace1(){\tfrac{b}{a}} - \pi & \quad \text{if $a < 0$ and $b < 0$}
    \\[0.5em]
    +\pi                                   & \quad \text{if $a < 0$ and $b = 0$}
    \\[0.5em]
    +\tfrac{\pi}{2}                        & \quad \text{if $a = 0$ and $b > 0$}
    \\[0.5em]
    -\tfrac{\pi}{2}                        & \quad \text{if $a = 0$ and $b < 0$}
    \\[0.5em]
    \mathrm{undefined}                     & \quad \text{if $a = 0$ and $b = 0$}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

For this definition, the principal value of the phase angle lies in the following range:

{% latex 1 fig-20 %}
    \begin{document}
    \begin{displaymath}
    -\pi < \phi \leq +\pi
    \end{displaymath}
    \end{document}
{% endlatex %}

We could have chosen a different range for the principal value, but this one seems to be the most widely used convention. In cases where both real and imaginary values are zero, the phase angle is undefined. It doesn't matter because the magnitude is zero. But if you're using a computer to calculate the phase, you might want to be careful what concrete result you get in this case. Consider what happens if you use the [.NET two-argument arctangent](https://docs.microsoft.com/en-us/dotnet/api/system.math.atan2) function to compute the phase when the real and imaginary coefficients are both zero:

{% latex 1 fig-21 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{4.5em}|wl{4.5em}|wl{4em+2.0555em}|}
    \hline
    a       & b       & \phi
    \\[0.25em]\hline
    +0.0000 & +0.0000 & +0.0000
    \\[0.25em]\hline
    +0.0000 & -0.0000 & -0.0000
    \\[0.25em]\hline
    -0.0000 & +0.0000 & +3.1416
    \\[0.25em]\hline
    -0.0000 & -0.0000 & -3.1416
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Do you see why I mentioned the signed zeros? You can't assume that a positive zero will always be treated the same as a negative zero. If you're not careful, you might wind up with some very unexpected results. I suspect other software libraries that provide mathematical functions have similar nuances. There are other subtleties related to this that are beyond the scope of this post. For now, let's just treat zero and near-zero complex values as having a phase angle of zero. Depicted below are the results of our Fourier transform expressed in polar form:

{% latex 1 fig-22 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|wl{3.5em}|wl{4.5em}|}
    \hline
    k & f_k    & A_k    & \phi_k
    \\[0.25em]\hline
    0 & 0.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    1 & 1.0000 & 0.5000 & -1.5708
    \\[0.25em]\hline
    2 & 2.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    3 & 3.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    4 & 4.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    5 & 5.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    6 & 6.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    7 & 7.0000 & 0.5000 & +1.5708
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

The magnitude component is always a positive or zero value, so we need not worry about the sign. Just like we did with the real and imaginary parts earlier, we can plot the values of the magnitude and phase components across the frequency domain:

{% chart fig-23-example-1-freq-ys-ma.svg %}
{% chart fig-24-example-1-freq-ys-ph.svg %}

Note the symmetry and reverse symmetry of the magnitude and phase components, respectively. We'll explore that in the next section. For the time being, let's just convert these values in the frequency domain back to the time domain using the inverse transform:

{% latex 1 fig-25 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    x(t) & = \sum_{k = 0}^{N - 1} y_k e^{i 2 \pi f_k t}
    \\[1em]
         & = \sum_{k = 0}^{N - 1} A_k e^{i \phi_k} e^{i 2 \pi f_k t}
    \\[1em]
         & = \sum_{k = 0}^{N - 1} A_k e^{i (2 \pi f_k t + \phi_k)}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

As shown above, we can easily consolidate the exponents when expressing our complex values in polar form. And using Euler's formula, we can take it a step further:

{% latex 1 fig-26 %}
    \begin{document}
    \begin{displaymath}
    x(t) =
    \sum_{k = 0}^{N - 1}
    A_k \1
    \brace3[]
    {
    \cos\2\brace1(){2 \pi f_k t + \phi_k}
    + i
    \sin\2\brace1(){2 \pi f_k t + \phi_k}
    }
    \end{displaymath}
    \end{document}
{% endlatex %}

Since the original values in the time domain are real numbers, not complex numbers, we can expect the results of the inverse transform to be real numbers as well. We can simply drop the imaginary part from the formula above:

{% latex 1 fig-27 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \sum_{k = 0}^{N - 1} A_k \cos\2\brace1(){2 \pi f_k t + \phi_k}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the inverse transform expressed in a form that uses the magnitude and phase coefficients. In this particular example, all but two of the values in the frequency domain are zero. Thus, we can further simplify like so:

{% latex 1 fig-28 %}
    \begin{document}
    \begin{displaymath}
    x(t) =
    A_1 \cos\2\brace1(){2 \pi f_1 t + \phi_1}
    +
    A_7 \cos\2\brace1(){2 \pi f_7 t + \phi_7}
    \end{displaymath}
    \end{document}
{% endlatex %}

If we plot this function on a chart and take discrete samples at the eight points in time we decided upon earlier, here is what it looks like:

{% chart fig-29-example-1-time-xs-repro.svg %}

This function is definitely not the same as the sine wave function we started with. However, it does have the same values as our original input function at the sample points. And that's the thing that matters when dealing with discrete Fourier transforms.

## Example 2

In the previous example, we noted the symmetry of the transformed values on the frequency domain. For the magnitude, the lower half of the frequency spectrum is a mirror image of the upper half. A similar pattern occurs with the phase angle, except the two halves are negated mirror images of one another. What happens if we consolidate the values onto the lower or upper half of the frequency spectrum? In this example, let's use the same input function we used in the previous section. We'll start with the transformed values on the frequency domain:

{% latex 1 fig-30 %}
    \begin{document}
    \begin{displaymath}
    y_k = \frac{1}{N} \sum_{n = 0}^{N - 1} \sin\2\brace1(){2 \pi t_n} \, e^{-i 2 \pi f_k t_n}
    \end{displaymath}
    \end{document}
{% endlatex %}

These are the same complex values on the frequency domain we saw in the previous section. Now let's perform some modifications to these values. We want to double the values on the lower half of the frequency spectrum and zero out the values on the upper half. But we want to avoid modifying the value for the zero frequency. We also want to avoid altering the middle value if the number of samples is an even number. To do this, it helps to define some demarcation points:

{% latex 1 fig-31 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    u & = \brace1{\lfloor}{\rfloor}{\tfrac{N - 1}{2}}
    \\[0.5em]
    v & = \brace1{\lfloor}{\rfloor}{\tfrac{N + 2}{2}}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since we are dividing integers here, we can use floor division to get integer results. These points give us an upper bound for the lower half of the range and a lower bound for the upper half. Here is how we modify the values on the frequency domain:

{% latex 1 fig-32 %}
    \begin{document}
    \begin{displaymath}
    y_k' =
    \begin{dcases}
    2 y_k           & \quad \text{if $1 \leq k \leq u$}
    \\[0.5em]
    \phantom{2} 0   & \quad \text{if $v \leq k \leq N$}
    \\[0.5em]
    \phantom{2} y_k & \quad \text{otherwise}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

This formula doubles the values in the lower part of the frequency range and clears the values in the upper part. Here is a visual representation of the modified values:

{% chart fig-33-example-2-freq-ys-lower-ma.svg %}
{% chart fig-34-example-2-freq-ys-lower-ph.svg %}

After performing the modification, all but one of the values in the frequency domain is a nonzero value. Here is what our inverse transform looks like in this instance:

{% latex 1 fig-35 %}
    \begin{document}
    \begin{displaymath}
    x(t) = A_1 \cos\2\brace1(){2 \pi f_1 t + \phi_1}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plotting this function on a chart with discrete samples at the predetermined sample points on the time domain, here is what it looks like:

{% chart fig-36-example-2-time-xs-lower.svg %}

This is identical to our original sine wave function. But the inverse transform gives us a formula based on the cosine function. In this case, the phase of the cosine wave is shifted by a quarter of a period, making it equivalent to a sine wave. We can reproduce the original input function using only values from the lower half of the frequency spectrum. But what happens if we only use values from the upper half of the frequency spectrum instead? Consider the following:

{% latex 1 fig-37 %}
    \begin{document}
    \begin{displaymath}
    y_k' =
    \begin{dcases}
    \phantom{2} 0   & \quad \text{if $1 \leq k \leq u$}
    \\[0.5em]
    2 y_k           & \quad \text{if $v \leq k \leq N$}
    \\[0.5em]
    \phantom{2} y_k & \quad \text{otherwise}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

This formula clears the values in the lower part of the frequency range and doubles the values in the upper part. Here is a visual representation of the modified values:

{% chart fig-38-example-2-freq-ys-upper-ma.svg %}
{% chart fig-39-example-2-freq-ys-upper-ph.svg %}

Analogous to the previous instance, all but one of the values in the frequency domain is a nonzero value. Here is what our inverse transform looks like in this instance:

{% latex 1 fig-40 %}
    \begin{document}
    \begin{displaymath}
    x(t) = A_7 \cos\2\brace1(){2 \pi f_7 t + \phi_7}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plotting this function on a chart with discrete samples at the predetermined sample points on the time domain, here is what it looks like:

{% chart fig-41-example-2-time-xs-upper.svg %}

As you can see, we have an entirely different function on the time domain when using only the upper frequencies. But the values at the sample points are still the same as when we used only the lower frequencies. Because the values on each half the frequency range are mirror images of one another, we only need half of these values to complete a full circle back to the time domain.

## Example 3

Suppose we have a function that is the superimposition of several different sinusoidal waves. And each one of those waves can have an amplitude, phase offset, and frequency value that is entirely independent of all the others. Here is a plot of our function:

{% chart fig-42-example-3-time-xs.svg %}

Now let's say that maybe we don't even know how this input function is defined; we just know the values at the samples points. Here are the samples values:

{% latex 1 fig-43 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|wl{8em+2.0555em}|}
    \hline
    n & t_n    & x_n
    \\[0.25em]\hline
    0 & 0.0000 & +0.1768
    \\[0.25em]\hline
    1 & 0.1250 & +0.3536
    \\[0.25em]\hline
    2 & 0.2500 & +0.8232
    \\[0.25em]\hline
    3 & 0.3750 & +1.8107
    \\[0.25em]\hline
    4 & 0.5000 & -0.8839
    \\[0.25em]\hline
    5 & 0.6250 & -1.0607
    \\[0.25em]\hline
    6 & 0.7500 & -0.1161
    \\[0.25em]\hline
    7 & 0.8750 & -1.1036
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using these samples values, we can apply the forward transform to find the equivalent representation on the frequency domain:

{% latex 1 fig-44 %}
    \begin{document}
    \begin{displaymath}
    y_k = \frac{1}{N} \sum_{n = 0}^{N - 1} x_n e^{-i 2 \pi f_k t_n}
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's assume the input function consists of sinusoidal waves with frequencies only on the lower half of the frequency spectrum. We can modify the values in the frequency domain as shown in the previous section:

{% latex 1 fig-45 %}
    \begin{document}
    \begin{displaymath}
    y_k' =
    \begin{dcases}
    2 y_k           & \quad \text{if $1 \leq k \leq u$}
    \\[0.5em]
    \phantom{2} 0   & \quad \text{if $v \leq k \leq N$}
    \\[0.5em]
    \phantom{2} y_k & \quad \text{otherwise}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a visual representation of the modified values in the frequency domain:

{% chart fig-46-example-3-freq-ys-ma.svg %}
{% chart fig-47-example-3-freq-ys-ph.svg %}

Here are the calculated results for each of the predefined frequencies:

{% latex 1 fig-48 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{1.5em}|wl{3.5em}|wl{3.5em}|wl{4.5em}|}
    \hline
    k & f_k    & A_k    & \phi_k
    \\[0.25em]\hline
    0 & 0.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    1 & 1.0000 & 1.0000 & -1.5708
    \\[0.25em]\hline
    2 & 2.0000 & 0.5000 & +2.3562
    \\[0.25em]\hline
    3 & 3.0000 & 0.7500 & -0.7854
    \\[0.25em]\hline
    4 & 4.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    5 & 5.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    6 & 6.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    7 & 7.0000 & 0.0000 & \pm 0.0000
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using these values in the frequency domain, we can now apply the inverse transform to produce a suitable function in the time domain:

{% latex 1 fig-49 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \sum_{k = 0}^{N - 1} A_k \cos\2\brace1(){2 \pi f_k t + \phi_k}
    \end{displaymath}
    \end{document}
{% endlatex %}

And since there are only three nonzero values in the table above, we can represent this function as the sum of three distinct sinusoidal waves:

{% latex 1 fig-50 %}
    \begin{document}
    \begin{displaymath}
    x(t) =
    A_1 \cos\2\brace1(){2 \pi f_1 t + \phi_1}
    +
    A_2 \cos\2\brace1(){2 \pi f_2 t + \phi_2}
    +
    A_3 \cos\2\brace1(){2 \pi f_3 t + \phi_3}
    \end{displaymath}
    \end{document}
{% endlatex %}

Thus, we have a function that is the superimposition of three different sinusoidal waves, each with a different amplitude, phase offset, and frequency. The following plots give a visual representation of each of these three sinusoidal components:

{% chart fig-51-example-3-time-xs-1.svg %}
{% chart fig-52-example-3-time-xs-2.svg %}
{% chart fig-53-example-3-time-xs-3.svg %}

Adding these three waveforms together gives us a function that reproduces our original input. The beauty of the discrete Fourier transform is that it allows us to decompose a function into discrete sinusoidal components. This can be useful if you want to amplify the values on some frequency ranges while attenuating the values on others. Adjustments can be made on the frequency domain that might otherwise be difficult or impossible to do on the time domain.

## Example 4

What happens if you shift a function vertically by some amount? How does this affect the values in the frequency domain? Consider the following input function:

{% latex 1 fig-54 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \sin\2\brace1(){2 \pi t} + 0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the same sine wave function we used in the first two examples, except it has been shifted upwards. Here is a plot of this function:

{% chart fig-55-example-4-time-xs.svg %}

Now we can apply the forward transform. Like we did in previous examples, let's assume we're only dealing with the frequencies on the lower part of the spectrum and modify the values accordingly. Here are the values on the frequency domain:

{% chart fig-56-example-4-freq-ys-ma.svg %}
{% chart fig-57-example-4-freq-ys-ph.svg %}

Notice the magnitude value for the zero frequency. This value represents the vertical shift in the upward direction. Applying the inverse transform, we have the following:

{% latex 1 fig-58 %}
    \begin{document}
    \begin{displaymath}
    x(t) =
    A_0 \cos\2\brace1(){2 \pi f_0 t + \phi_0}
    +
    A_1 \cos\2\brace1(){2 \pi f_1 t + \phi_1}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now let's plug in the concrete values and simplify, taking into consideration the following:

{% latex 1 fig-59 %}
    \begin{document}
    \begin{displaymath}
    \cos\2(0) = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the simplified version of the inverse transform:

{% latex 1 fig-60 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \cos\2\brace1(){2 \pi t - 0.5 \pi} + 0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

The cosine of zero is one. Effectively, this means that a cosine wave with a frequency of zero is a horizontal line. Perhaps you can think of it as a sinusoidal wave with an infinite wavelength. In this case, the horizontal line is scaled by the magnitude, giving us our vertical offset.

## Example 5

We've seen what happens if you shift a function vertically in the upward direction. But what if you move it down instead of up? Consider the following input function:

{% latex 1 fig-61 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \sin\2\brace1(){2 \pi t} - 0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

Here we have the same sine wave function used in the previous example, except it has been shifted downwards instead of upwards. Here is a plot of this function:

{% chart fig-62-example-5-time-xs.svg %}

Now we can apply the forward transform. Like we did in previous examples, let's assume we're only dealing with the frequencies on the lower part of the spectrum and modify the values accordingly. Here are the values on the frequency domain:

{% chart fig-63-example-5-freq-ys-ma.svg %}
{% chart fig-64-example-5-freq-ys-ph.svg %}

Notice the phase offset for the zero frequency in addition to the magnitude value. This phase value reverses the vertical shift to the downward direction. Here is the inverse transform:

{% latex 1 fig-65 %}
    \begin{document}
    \begin{displaymath}
    x(t) =
    A_0 \cos\2\brace1(){2 \pi f_0 t + \phi_0}
    +
    A_1 \cos\2\brace1(){2 \pi f_1 t + \phi_1}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now let's plug in the concrete values and simplify, taking into consideration the following:

{% latex 1 fig-66 %}
    \begin{document}
    \begin{displaymath}
    \cos\2(\pi) = -1
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the simplified version of the inverse transform:

{% latex 1 fig-67 %}
    \begin{document}
    \begin{displaymath}
    x(t) = \cos\2\brace1(){2 \pi t - 0.5 \pi} - 0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

As mentioned previously, a cosine wave with a zero frequency is essentially a horizontal line. But in this case, the phase is offset by half of a period, giving us a negative value. This value, scaled by the magnitude, gives us the appropriate offset in the downward direction.

## Example 6

Suppose we have a function that cannot be represented as the sum of a finite number of sinusoidal waves. A sloping line might be an example of such a function. What would the values on the frequency domain look like? What would happen if we transformed them back to the time domain? Consider this function as our example:

{% latex 1 fig-68 %}
    \begin{document}
    \begin{displaymath}
    x(t) = 2.5 t - 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a visual representation of this function, along with the sample values at the sample points on the time domain:

{% chart fig-69-example-6-time-xs.svg %}

Using the sample values, we can apply the forward transform the same way we did in the previous examples. And like we did before, let's modify the values in the frequency domain so that we're only using the frequencies in the lower part of the spectrum. Here is the result:

{% chart fig-70-example-6-freq-ys-ma.svg %}
{% chart fig-71-example-6-freq-ys-ph.svg %}

Once we have computed the values on the frequency domain, we can turn around and apply the inverse transform to complete a round trip back to the time domain. Here is the result of the inverse transform:

{% chart fig-72-example-6-time-xs-repro.svg %}

In this case, we cannot restore the original input function based on the values in the frequency domain. But as you can see from the chart above, we can surely approximate it. And we can do so in a way that reproduces the sample values at the sample points in the time domain. However, the sudden drop-off on the right-hand side of the chart is something worth taking note of. The approximation doesn't seem to lend itself to forecasting if the data isn't inherently sinusoidal.

## Fast Fourier Transforms

In the examples above, we used a naive method for computing the discrete Fourier transform and its corresponding inverse transform. While simple and easy to understand, this is probably not the most efficient computation method. There are a variety of fast Fourier transform algorithms that are likely to be much more computationally efficient. And there are preexisting software libraries that implement them. Some of these libraries are even designed to run on GPU hardware. If you're working with larger data sets, using one of these fast implementations can significantly improve runtime performance.

{% accompanying_src_link %}
