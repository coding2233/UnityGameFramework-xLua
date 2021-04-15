#!/usr/bin/env bash
set -e
set -x

echo "Building for $BUILD_TARGET"
echo "Building path $BUILD_PATH"

# export BUILD_PATH=$UNITY_DIR/build/asset/$BUILD_TARGET/
mkdir -p $BUILD_PATH

  $1 \
  -projectPath $UNITY_DIR \
  -quit \
  -batchmode \
  -nographics \
  -executeMethod $2 \
  -logFile $UNITY_DIR/build_log.txt \
  -customBuildTarget $BUILD_TARGET

cat $UNITY_DIR/build_log.txt


UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

rm $UNITY_DIR/build_log.txt

ls -lha $BUILD_PATH
[ -n "$(ls -A $BUILD_PATH)" ] # fail job if build folder is empty