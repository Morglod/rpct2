NAME="RPCT_REPEAT";
MAX=128;

echo "#define RPCT_REPEAT_MAX $MAX"
echo "#define ${NAME}_1(EXPR) EXPR(1)"
for i in {2..128}; do
    j=$((i-1));
    echo "#define ${NAME}_$i(EXPR) ${NAME}_$j(EXPR) EXPR($i)"
done

echo "#define $NAME(N, EXPR) ${NAME}_##N(EXPR)"

NAME="RPCT_REPEAT_J";
echo "#define ${NAME}_1(EXPR) EXPR(1)"
for i in {2..128}; do
    j=$((i-1));
    echo "#define ${NAME}_$i(EXPR) ${NAME}_$j(EXPR) EXPR($i)"
done

echo "#define $NAME(N, EXPR) ${NAME}_##N(EXPR)"

NAME="RPCT_REPEAT_K";
echo "#define ${NAME}_1(EXPR) EXPR(1)"
for i in {2..128}; do
    j=$((i-1));
    echo "#define ${NAME}_$i(EXPR) ${NAME}_$j(EXPR) EXPR($i)"
done

echo "#define $NAME(N, EXPR) ${NAME}_##N(EXPR)"
