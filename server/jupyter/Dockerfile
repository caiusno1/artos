FROM jupyter/base-notebook

LABEL maintainer="Kai Biermeier <kbiermeier1@web.de>"
ARG NB_USER="jovyan"
ARG NB_UID="1000"
ARG NB_GID="100"
ENV DEBIAN_FRONTEND noninteractive
# Configure environment
ENV CONDA_DIR=/opt/conda \
    SHELL=/bin/bash \
    NB_USER="${NB_USER}" \
    NB_UID=${NB_UID} \
    NB_GID=${NB_GID} \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    LANGUAGE=en_US.UTF-8
ENV PATH="${CONDA_DIR}/bin:${PATH}" \
    HOME="/home/${NB_USER}"
RUN conda install -c conda-forge jupyterlab
RUN conda create -n py39 python=3.9
USER root
RUN apt update
RUN apt install -y git
RUN apt install -y g++
USER ${NB_UID}
SHELL ["conda", "run", "-n", "py39", "/bin/bash", "-c"]
RUN conda install -c conda-forge ipykernel pymc3 theano-pymc mkl mkl-service blas -y
RUN pip install git+https://github.com/mwaskom/seaborn.git@069d53a3dd29a965987705c823883a93700bbe5e
RUN pip install pandas
RUN pip install matplotlib
RUN python -m ipykernel install --name=py39 --user
WORKDIR "${HOME}"
ENTRYPOINT ["tini", "-g", "--"]
CMD ["start-notebook.sh"]