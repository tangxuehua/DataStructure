 Void Mergelist_Sq(SqList La,SqList Lb,Sqlist &Lc)
    //已知顺序线性表La和Lb的元素按值非递减顺序排列
    //合并La和Lb的元素，得到新顺序表Lc,使Lc也递增
 {
    i=j=0;k=0;
    while（i<La.length && j<Lb.length）
    {
       If(La.elem[i]<=Lb.elem[j])
  	     Lc.elem[k++]=La.elem[i++];
       Else
           Lc.elem[k++]=Lb.elem[j++];
　　}
　　While(i<La.length)   //插入La中的剩余元素
　　   Lc.elem[k++]=La.elem[i++];
　　While(j<Lb.length)   //插入Lb中的剩余元素
　　   Lc.elem[k++]=Lb.elem[j++];
　　Lc.length=k;
 }  //MergeList_Sq
