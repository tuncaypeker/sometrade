﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SomeTrade.Indicators.LazyBear
{
    /*
    
    //
    // @author LazyBear 
    // List of all my indicators: 
    // https://docs.google.com/document/d/15AGCufJZ8CIUvwFJ9W-IKns88gkWOKBCvByMEvm5MLo/edit?usp=sharing
    // 
    study(title="Variable Moving Average [LazyBear]", shorttitle="VMA_LB", overlay=true)
    src=close
    l =input(6, title="VMA Length") 
    std=input(false, title="Show Trend Direction")
    bc=input(false, title="Color bars based on Trend")
    k = 1.0/l
    pdm = max((src - src[1]), 0)
    mdm = max((src[1] - src), 0)
    pdmS = ((1 - k)*nz(pdmS[1]) + k*pdm)
    mdmS = ((1 - k)*nz(mdmS[1]) + k*mdm)
    s = pdmS + mdmS
    pdi = pdmS/s
    mdi = mdmS/s
    pdiS = ((1 - k)*nz(pdiS[1]) + k*pdi)
    mdiS = ((1 - k)*nz(mdiS[1]) + k*mdi)
    d = abs(pdiS - mdiS)
    s1 = pdiS + mdiS
    iS = ((1 - k)*nz(iS[1]) + k*d/s1)
    hhv = highest(iS, l) 
    llv = lowest(iS, l) 
    d1 = hhv - llv
    vI = (iS - llv)/d1
    vma = (1 - k*vI)*nz(vma[1]) + k*vI*src
    vmaC=(vma > vma[1]) ? green : (vma<vma[1]) ? red : (vma==vma[1]) ? blue : black 
    plot(vma, color=std?vmaC:black, linewidth=3, title="VMA")
    barcolor(bc?vmaC:na)

    kivancin yorumları
    : Değişen hareketli ortalma VIDYA[Variable Index Dynamic Moving Average] ile karışmasın
    : Volatileye göre otomatik yumuşatma yapar
    : Düşük periyolatda, [bar uzunluklarında-mesela 10] daha anlamlı sonuclar, 
    : Büyük periyotta kullanma

     */ 
    class VMA_LB
    {
    }
}
