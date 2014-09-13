void InsertSort(SSTable R[],int n){
	int i,j;
	for(i = 2;i <= n;i++){
		R[0] = R[i]; j = i - 1;
		while(R[0].key < R[j].key){
			R[j+1] = R[j];
			j--;
		}
		R[j+1] = R[0];
	}
	return;
}
