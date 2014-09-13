void BubbleSort(SSTable R[],int n){
	int i,j,flag;
	for(i = 1;i < n; i++){
		flag = 1;   //用来判断排序是否提前结束
		for(j = 1;j <= n - i;j++){
			if(R[j+1].key < R[j].key){
				flag = 0;   //表明排序还没有结束
				R[0] = R[j];
				R[j] = R[j+1];
				R[j+1] = R[0];
			}
		}
		if(flag == 1){
			return;
		}
	}
	return;
}
