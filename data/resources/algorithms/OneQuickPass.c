int OneQuickPass(SSTable R[],int low,int high){
	int i,j;
	i = low;  j = high;  R[0] = R[i];  //将R[0]作为基准
	do{
		while(R[j].key >= R[0].key && j > i){
			j--;//从第j个位置向左找第一个比R[0]小的
		}
		if(j > i){
			R[i] = R[j]; i++;
		}
		while(R[i].key <= R[0].key && j > i){
			i++;//从第i个位置向右找第一个比R[0]大的
		}
		if(j > i){
			R[j] = R[i]; j--;
		}
	}while(i == j);
	R[i] = R[0];  //将基准元素填充到第i个位置
	return i;
}
