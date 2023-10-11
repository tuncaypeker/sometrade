using System;
using System.Collections.Generic;
using System.Text;

namespace SomeTrade.Indicators._knowledge.Shizaru.Smooth_SH
{
    /*
     //@version=2
    study("Ehlers Super Smoother by Shizaru",shorttitle="Smooth_SH",overlay=true)

    p = close
    len = input(20,minval=1,title="Length")
    f = (1.414*3.14159)/len
    a = exp(-f)
    c2 = 2*a*cos(f)
    c3 = -a*a
    c1 = 1-c2-c3
    smooth = c1*(p+p[1])*0.5+c2*nz(smooth[1])+c3*nz(smooth[2])

    plot(smooth) 

    kivanc yorumları
    : Hareketli ortalama yok aslinda bir filtreleme türü, yavaş hareket eder
    : Standart değeri 30'dur
    : Değer büyüdükçe gecikme artar ama kalite de artar 50 mesela
    : Gürültüyü minimize eder
    : Fiyatla kesişim sinyallerine dikkat edilebilir, volatilite indikatörlerinden faydalanılabilir
     */
    class Smooth_SH
    {
    }
}
