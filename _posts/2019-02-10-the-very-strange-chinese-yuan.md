---
layout: post
title: The Very Strange Chinese Yuan
---

In my previous post, I explored the distribution of price fluctuations for a variety of different markets and time frames. Across all data sets, plotting the log returns in a histogram appears to roughly approximate the density function of a Laplace distribution. The intraday prices of the Chinese yuan, however, seem to exhibit a distinctly strange phenomenon.

<!--excerpt-->

The exchange rate between the Chinese yuan and the US dollar is not an entirely market driven rate the way many other currency exchange rates are. While the value of the yuan is allowed to float to some degree, the People's Bank of China maintains a certain level of control over the yuan's exchange rate. Furthermore, the yuan is quoted in two separate markets: an onshore market and an offshore market.

## The Onshore Exchange Rate

The onshore exchange rate, according to my understanding, is determined by a reference rate set daily by China's central bank. The exchange rate is allowed to float within a narrow band above or below the reference rate. Let's look at an intraday chart of the onshore exchange rate between the Chinese yuan and the US dollar over a period of four days:

{% chart fig-01-forex-intraday-USDCNY-price-lin.svg %}

The chart above is based on one minute intraday data. This chart looks a bit odd to me. It seems to be sprinkled with unnatural spikes that quickly revert back to their previous values. The price doesn't zigzag the same way prices seem to move on other price charts. But this is just a subject observation. Let's look at a histogram of price differences from one minute to the next:

{% chart fig-02-forex-intraday-USDCNY-probs-lin.svg %}
{% chart fig-03-forex-intraday-USDCNY-probs-log.svg %}

The two charts above show the same data; one plots the density on a linear scale and the other shows it on a logarithmic scale. Employing the same technique used in my previous post, the histogram is overlaid with the density functions of the fitted normal and Laplace distributions. Notice that neither one of these two fitted distributions seem to model the shape of the histogram.

If you were to only look at the centermost third of the chart, it would seem like the distribution of price fluctuations might conform to that of a Laplace distribution just like the data sets examined previously. However, the outermost thirds of the chart tell a different story. There is an outsized cluster of large moves up and down.

## The Offshore Exchange Rate

The offshore exchange rate is supposedly a floating rate determined by the market. While I'm no expert on the specific details, I presume the Chinese central bankers exert some form of indirect influence on the offshore rate to keep it in parity with the onshore rate. Here is a one minute intraday chart of the offshore rate covering a period 24 hours:

{% chart fig-04-forex-intraday-USDCNH-price-lin.svg %}

This one also looks odd to me. It has those same unnatural looking spikes, mostly in the downward direction. If we examine distribution of price movements for the offshore rate, will the histogram exhibit the same triple peak pattern as the onshore rate? Take a look:

{% chart fig-05-forex-intraday-USDCNH-probs-lin.svg %}
{% chart fig-06-forex-intraday-USDCNH-probs-log.svg %}

Indeed, this strange phenomenon exists for both the offshore rate and the onshore rate. I don't have a solid explanation for this. It might simply be an error or anomaly introduced by my data provider. But I suspect it's related to whatever mechanisms the central bankers are using to put floors and ceilings on the yuan's exchange rate. I have also observed this phenomenon in the US dollar exchange rate with the Hong Kong dollar and, to a lesser extent, the Taiwan dollar.

## The Euro Exchange Rate

Does the exchange rate between the yuan and the euro exhibit the same characteristics as the exchange rate between the yuan and the US dollar? Here is a chart of the offshore exchange rate between the yuan and the euro:

{% chart fig-07-forex-intraday-EURCNH-price-lin.svg %}

This data set covers the same 24 hour period as the US dollar offshore rate examined in the previous section. This price chart seems to be much cleaner, with fewer of those unnatural price spikes. The distribution of price fluctuations looks like this:

{% chart fig-08-forex-intraday-EURCNH-probs-lin.svg %}
{% chart fig-09-forex-intraday-EURCNH-probs-log.svg %}

The triple peak pattern is not present in this data set. I wasn't expecting this. Like many other data sets examined in my previous post, the distribution of price fluctuations for these data roughly approximates a Laplace distribution.

## Synthetic Exchange Rates

Without observing a quoted exchange rate directly, a synthetic exchange rate between two currencies can be computed from the rates of two other currency pairs. Suppose we know the offshore exchange rate between the yuan and the euro. And suppose we also know the exchange rate of the euro against the dollar. We can compute the offshore exchange rate between the yuan and the dollar synthetically using the euro as an intermediary. Here is the result of this computation:

{% chart fig-10-pair-synthetic-USDCNH-price-lin.svg %}

As expected, this chart looks very similar to the intraday chart of the offshore rate between the yuan and the dollar shown earlier. In fact, I think this chart looks even better than the one based on the actual quoted values. It doesn't have as many of those unnatural looking spikes. Here is what the distribution of price movements looks like:

{% chart fig-11-pair-synthetic-USDCNH-probs-lin.svg %}
{% chart fig-12-pair-synthetic-USDCNH-probs-log.svg %}

The histogram based on the synthetic data does not show a trace of the triple peak pattern visible in the histogram based on the actual quoted values. Perhaps there is an arbitrage opportunity here worth exploring.

## Daily Exchange Rates

When analyzing the intraday exchange rates between the yuan and the dollar, we observed a triple peak pattern in the distribution of price fluctuations. Does this triple peak pattern manifest itself if we examine daily data instead of intraday day? Here is the analysis of the onshore exchange rate using daily data:

{% chart fig-13-forex-daily-USDCNY-probs-lin.svg %}
{% chart fig-14-forex-daily-USDCNY-probs-log.svg %}

It does not exhibit the triple peak pattern characteristic of intraday prices. The histogram takes the shape of a Laplace distribution, which is the pattern we see with other data sets. Here is the analysis of the offshore exchange rate using daily data:

{% chart fig-15-forex-daily-USDCNH-probs-lin.svg %}
{% chart fig-16-forex-daily-USDCNH-probs-log.svg %}

Again, we see the same thing. The histogram takes the approximate shape of a Laplace distribution. The daily price fluctuations of both the onshore and offshore exchange rates exhibit the same behavior we have come to expect from many other data sets. The triple peak phenomenon only seems to only manifest itself in smaller timeframes.
