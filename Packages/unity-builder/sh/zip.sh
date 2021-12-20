# ============================================
# 작성자 : Strix
# 주의사항 : Window에서 파일 수정후 맥에서 실행시 줄바꿈 코드(EOL)를 Unix 형식으로 변경바랍니다. (Window/MAC은 안됨)
# ============================================


ZIP_PATH=$1
ZIP_FILE_NAME=$2

cd $ZIP_PATH
zip -r $ZIP_FILE_NAME *