Status IndexBF(SString S,SString T,int pos){
    //返回串T在主串S中第pos个字符之后的位置。若不存在
    //则函数返回0；其中，T非空，1<=pos<=StrLength(S)
    i=pos;  j=1;
    While(i<=s[0] && j<=T[0]){    //注：S[0]和T[0]分别存放其字符串长度
        if(S[i]==T[j])
            {i++;  j++}
        else
            {i=i-j+2;  j=1}       //当发现不匹配时，i返回前一次匹配
    }                             //的起始字符的后续字符，j返回第一个字符。
    if(j>T[0])
        return   i-T[0];           //匹配成功，返回相应位置
    else
        return 0;
}  //IndexBF
